using UnityEngine;

/// <summary>
/// Every item is affected by gravity. It ca be picked up, thrown, dropped and activated. 
/// This base class can be enhanced to add custom functionality on top of the base behaviour.
/// </summary>
public class Item : MonoBehaviour{

    protected Rigidbody2D rgbd;
    protected Collider2D col;
    public bool isInteractable;
    public virtual void Start(){
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    public virtual bool TryPickup(Transform carryPoint){
        if(isInteractable){
            rgbd.simulated = false;
            col.enabled = false;
            transform.SetParent(carryPoint);
            transform.localPosition = Vector3.zero;
            return true;
        }
        else{
            return false;
        }

    }
    public virtual void Throw(Vector2 throwDirection, float throwForce){
        transform.SetParent(null);
        rgbd.simulated = true;
        col.enabled = true;
        rgbd.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }
    public virtual void Drop(){
        transform.SetParent(null);
        rgbd.simulated = true;
        col.enabled = true;
    }
    public virtual bool TryActivate(){
        // TODO: Implement a fitting base behaviour
        if(isInteractable){
            return true;
        }
        else{
            return false;
        }
    }

    public void SetInteractable(bool isInteractable){
        this.isInteractable = isInteractable;
    }
}