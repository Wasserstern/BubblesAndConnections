using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //References
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
    public float superJumpForce;
    public float superJumpForceFactor;
    public float minSuperJumpLoadTime;
    public float maxSuperJumpLoadTime;

    //Runtime variables
    float xInput;
    float yInput;
    [SerializeField]
    bool isHoldingSuperJump;
    [SerializeField]
    bool isPressingSit;
    [SerializeField]
    bool isGrounded;
    [SerializeField]
    bool isSitting;
    public Transform currentBubbleTransform;
    Vector2 standardGravity;
    public float standardDrag;
    [SerializeField]
    float superJumpLoadTime;

    void Start()
    {
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
        xInput = !isSitting ?  Input.GetAxis("Horizontal") : 0f;
        yInput = Input.GetAxisRaw("Vertical");
        isHoldingSuperJump = Input.GetKey(KeyCode.Space);
        isPressingSit = Input.GetKeyDown(KeyCode.LeftControl);
        Vector2 groundCheckDirection = (downDirector.position - transform.position).normalized;
        Debug.DrawRay(downDirector.transform.position, groundCheckDirection * groundCheckDistance, Color.green, 0.3f);
        isGrounded = Physics2D.Raycast(downDirector.transform.position, groundCheckDirection, groundCheckDistance, LayerMask.GetMask("Ground"));
        
        Debug.Log(isGrounded);

        // Set animator variables
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isMoving", xInput > 0f || xInput < 0f);
        animator.SetBool("isHoldingSuperJump", isHoldingSuperJump);

        if(!isSitting && isPressingSit &&isGrounded && xInput == 0f && rgbd.velocity.magnitude != 0.2f ){
            // Sitdown
            isSitting = true;
            rgbd.velocity = Vector3.zero;
        }
        animator.SetBool("isSitting", isSitting);

        if(yInput > 0){
            if(isSitting){
                isSitting = false;
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

        if(isGrounded){
            if(isHoldingSuperJump)
            {
                superJumpLoadTime += Time.deltaTime;
                if(superJumpLoadTime > maxSuperJumpLoadTime){
                    rgbd.velocity = Vector2.zero;
                    superJumpLoadTime = maxSuperJumpLoadTime;
                }
            }
            else
            {
                if(superJumpLoadTime > minSuperJumpLoadTime){
                    rgbd.AddForce(transform.up * superJumpForce * superJumpLoadTime / superJumpForceFactor, ForceMode2D.Impulse);
                }
                superJumpLoadTime = 0f;
            }
        }
        else{
            superJumpLoadTime = 0f;
        }
        isGrounded = Physics2D.Raycast(transform.position, groundCheckDirection, groundCheckDistance, LayerMask.GetMask("Ground"));

    }
    void FixedUpdate(){
        
        if(currentBubbleTransform != null){
            // Gravity and force handling when inside bubble gravity range
            Vector2 bubbleDirection = ((Vector2)currentBubbleTransform.position - (Vector2)transform.position).normalized;
            Physics2D.gravity = bubbleDirection * standardGravity.magnitude;
            rgbd.drag = standardDrag;

            if(!isGrounded && rgbd.velocity.magnitude < maxFallMagnitude)
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
