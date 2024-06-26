using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class PlayerMovement : MonoBehaviour
{

    //References
    PlayerState playerState;
    Rigidbody2D rgbd;
    Collider2D col;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Transform downDirector;

    // General settings
    public float groundCheckDistance;

    //Movement settings
    public float moveAcceleration;
    public float moveDeceleration;
    public float moveDecelerationFast;
    public float maxMoveMagnitude;
    public float minMoveMagnitude;
    public float fallAcceleration;
    public float maxFallMagnitude;
    public float jumpForce;
    public float jumpTime;
    public float fallTime;
    public float startJumpGravityScale;
    public float minJumpGravityScale;
    public float maxJumpGravityScale;
    //Runtime variables
    float xInput;
    float yInput;
    public Transform currentBubbleTransform;
    Vector2 standardGravity;
    public float standardDrag;


    void Start()
    {
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        standardGravity = Physics2D.gravity;
        downDirector = transform.GetChild(0);
    }

    void Update()
    {
        // Get inputs and check ground
        xInput = !playerState.isSitting ?  Input.GetAxis("Horizontal") : 0f;
        yInput = Input.GetAxisRaw("Vertical");
        playerState.isPressingSit = Input.GetKeyDown(KeyCode.LeftControl);
        playerState.isPressingJump = Input.GetKeyDown(KeyCode.Space);
        playerState.isHoldingJump = Input.GetKey(KeyCode.Space);
        Vector2 groundCheckDirection = (downDirector.position - transform.position).normalized;
        Debug.DrawRay(downDirector.transform.position, groundCheckDirection * groundCheckDistance, Color.green, 0.3f);
        playerState.isGrounded = Physics2D.Raycast(downDirector.transform.position, groundCheckDirection, groundCheckDistance, LayerMask.GetMask("Ground"));

        // Set animator variables
        animator.SetBool("isGrounded", playerState.isGrounded);
        animator.SetBool("isMoving", xInput > 0f || xInput < 0f);
        animator.SetBool("isJumping", playerState.isJumping);

        if(!playerState.isSitting && playerState.isPressingSit && playerState.isGrounded && xInput == 0f && rgbd.velocity.magnitude != 0.2f ){
            // Sitdown
            playerState.isSitting = true;
            rgbd.velocity = Vector3.zero;
        }
        animator.SetBool("isSitting", playerState.isSitting);

        if(yInput > 0){
            if(playerState.isSitting){
                playerState.isSitting = false;
            }
        }

        if(xInput > 0){
            spriteRenderer.flipX = false;
        }
        else if(xInput < 0){
            spriteRenderer.flipX = true;
        }

        if(currentBubbleTransform != null){
            // Set rotation towards target bubble 
            Vector2 bubbleDirection = ((Vector2)currentBubbleTransform.position - (Vector2)transform.position).normalized;
            Vector3 lookAtPosition = new Vector3(transform.position.x - bubbleDirection.x, transform.position.y - bubbleDirection.y, 0f);
            transform.up = lookAtPosition - transform.position;
        }
        else{
            // Reset rotation to zero
            Vector3 lookAtPosition = new Vector3(transform.position.x, transform.position.y +1f);
            transform.up = lookAtPosition - transform.position;
        }
        if(playerState.isGrounded){
            if(playerState.isPressingJump && !playerState.isJumping){
                playerState.isJumping = true;
                StartCoroutine(Jump());
            }
        }

    }

    IEnumerator Jump(){
        float initialGravityScale = rgbd.gravityScale;
        rgbd.gravityScale = startJumpGravityScale;
        rgbd.velocity = Vector3.zero;
        float startTime = Time.time;
        float elapsedTime = 0f;
        rgbd.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        while(playerState.isHoldingJump && Time.time - startTime < jumpTime){
            // Decrease gravity scale over time
            float t = EaseFunctions.easeOutBack(elapsedTime / jumpTime);
            rgbd.gravityScale = Mathf.Lerp(startJumpGravityScale, minJumpGravityScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        startTime = Time.time;
        elapsedTime = 0f;

        while(Time.time - startTime < fallTime){
            float t = EaseFunctions.easeInExpo(elapsedTime / jumpTime);
            rgbd.gravityScale = Mathf.Lerp(minJumpGravityScale, initialGravityScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rgbd.gravityScale = initialGravityScale;
        playerState.isJumping = false;
    }

    void FixedUpdate(){
        
        if(currentBubbleTransform != null){
            // Gravity and force handling when inside bubble gravity range
            Vector2 bubbleDirection = ((Vector2)currentBubbleTransform.position - (Vector2)transform.position).normalized;
            Physics2D.gravity = bubbleDirection * standardGravity.magnitude;
            rgbd.drag = standardDrag;

            if(!playerState.isJumping && !playerState.isGrounded && rgbd.velocity.magnitude < maxFallMagnitude)
            {
                rgbd.AddForce(Physics2D.gravity.normalized * fallAcceleration, ForceMode2D.Force);
            }
            if(xInput != 0f)
            {
                rgbd.AddForce(xInput * transform.right * moveAcceleration, ForceMode2D.Force);
                
            }
            else{
                if(rgbd.velocity.magnitude > minMoveMagnitude && currentBubbleTransform != null)
                {
                    rgbd.AddForce(new Vector2(-rgbd.velocity.x, 0f) * moveDeceleration, ForceMode2D.Force);
                }
            }
        }
        else{
            // Outside of bubble gravity range
            Physics2D.gravity = new Vector2(0, 0);
            rgbd.drag = 0f;
        }
        
        if(rgbd.velocity.magnitude > maxMoveMagnitude){
            float magnitudeExcess = rgbd.velocity.magnitude - maxMoveMagnitude;
            rgbd.AddForce(-rgbd.velocity.normalized * magnitudeExcess * moveDecelerationFast);
        }


        
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            currentBubbleTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            currentBubbleTransform = null;
        }
    }
}
