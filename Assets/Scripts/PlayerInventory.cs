using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // References
    PlayerState playerState;
    public Transform pickupMarker;
    public Transform carryPoint;
    public Transform throwDirector;
    SpriteRenderer pickupRenderer;
    public float pickupRadius;
    public float throwForce;
    Vector2 pickupDirection;
    [SerializeField]
    GameObject targetItem;
    GameObject carriedItem;
    float throwDirectorXOffset;
    void Start()
    {
        playerState = GetComponent<PlayerState>();
        pickupRenderer = pickupMarker.GetComponent<SpriteRenderer>();
        pickupDirection = (new Vector2(transform.position.x +1f, transform.position.y) - (Vector2)transform.position).normalized;
        throwDirectorXOffset = throwDirector.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input  = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(input != Vector2.zero)
        {
            pickupDirection = ((Vector2)transform.position + input - (Vector2)transform.position).normalized;
            throwDirector.localPosition = input.x > 0 ? new Vector3(throwDirectorXOffset ,throwDirector.localPosition.y, throwDirector.localPosition.z)
                                                      : new Vector3(-throwDirectorXOffset ,throwDirector.localPosition.y, throwDirector.localPosition.z);
        }
        if(playerState.isGrounded){
            // Get closest item 
            Collider2D[] itemCollidersInRange = Physics2D.OverlapCircleAll((Vector2)transform.position + pickupDirection, pickupRadius, LayerMask.GetMask("Item"));
            float minRange = 99999f;
            Debug.DrawRay(transform.position, pickupDirection * pickupRadius, Color.yellow, 5f);
            GameObject closestItem = null;
            foreach(Collider2D itemCollider in itemCollidersInRange){
                float distance = Vector3.Distance(transform.position, itemCollider.transform.position);
                if(distance < minRange){
                    closestItem = itemCollider.gameObject;
                    minRange = distance;
                }
            }
            targetItem = closestItem;

            if(targetItem != null && carriedItem == null && Input.GetKeyDown(KeyCode.J)){
                // Pickup item
                Debug.Log($"Target item null? {targetItem == null}");
                Debug.Log($"Carried item null? {carriedItem == null}");
                carriedItem = targetItem;
                Debug.Log(carriedItem.GetComponent<Item>());
                carriedItem.GetComponent<Item>().Pickup(carryPoint);
            }
        }
        else{
            targetItem = null;
        }

        if(Input.GetKeyDown(KeyCode.K) && carriedItem != null)
        {
            // TODO: Implement throwing here
            Vector3 throwDirection = (throwDirector.transform.position - carryPoint.transform.position).normalized;
            carriedItem.GetComponent<Item>().Throw(throwDirection, throwForce);
            carriedItem = null;
        }

        // Set visibility of pickup marker
        if(targetItem != null){
            pickupRenderer.enabled = true;
            pickupMarker.transform.position = targetItem.transform.position;
        }
        else{
            pickupRenderer.enabled = false;
        }
    }
}
