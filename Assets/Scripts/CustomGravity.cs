using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [Header("Custom Gravity Settings:")]
    public Vector2 gravityCenter;
    public float gravityForce;

    Rigidbody2D rgbd;
    Vector2 currentGravityDirection;
    float initialGravityForce;
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        initialGravityForce = gravityForce;
        // Set this objects gravity to be unaffected by global gravity
        rgbd.gravityScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        currentGravityDirection = (gravityCenter - new Vector2(transform.position.x, transform.position.y)).normalized;
        
        rgbd.AddForce(currentGravityDirection * gravityForce);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            gravityCenter = other.gameObject.transform.position;
            gravityForce = initialGravityForce;
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            gravityForce = 0f;
        }
    }
}
