using UnityEngine;

public class Item : MonoBehaviour{

    protected Rigidbody2D rgbd;
    protected Collider2D col;
    protected virtual void Start(){
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    public virtual void Pickup(Transform carryPoint){
        rgbd.simulated = false;
        col.enabled = false;
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
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
    public virtual void Activate(){
        // TODO: Implement a fitting base behaviour
    }
}