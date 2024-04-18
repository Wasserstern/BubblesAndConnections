using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //References
    Rigidbody2D rgbd;
    Collider2D col;

    //Settings
    public float moveAcceleration;
    public float moveDeceleration;
    public float moveDecelerationFast;
    public float maxMoveMagnitude;
    public float minMoveMagnitude;


    //Runtime variables
    float xInput;
    bool isPressingJump;
    public Transform currentBubbleTransform;
    Vector2 standardGravity;

    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        standardGravity = Physics2D.gravity;
    }

    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        isPressingJump = Input.GetKey(KeyCode.Space);

        if(currentBubbleTransform != null){
            Vector2 bubbleDirection = ((Vector2)currentBubbleTransform.position - (Vector2)transform.position).normalized;
            Vector3 lookAtPosition = new Vector3(transform.position.x - bubbleDirection.x, transform.position.y - bubbleDirection.y, 0f);
            transform.up = lookAtPosition - transform.position;

            
        }
        else{
            
        }
    }
    void FixedUpdate(){
        if(currentBubbleTransform != null){
            Vector2 bubbleDirection = ((Vector2)currentBubbleTransform.position - (Vector2)transform.position).normalized;
            Physics2D.gravity = bubbleDirection * Physics2D.gravity.magnitude;
        }
        else{
            Physics2D.gravity = standardGravity;
        }

        if(xInput != 0f){
            rgbd.AddForce(xInput * transform.right * moveAcceleration, ForceMode2D.Force);
        }
        else{
            if(rgbd.velocity.magnitude > minMoveMagnitude){
                rgbd.AddForce(-rgbd.velocity.normalized * moveDeceleration, ForceMode2D.Force);
            }
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
            currentBubbleTransform = other.transform;
        }
    }
}
