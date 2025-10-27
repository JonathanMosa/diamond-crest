using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WindZone2D : MonoBehaviour
{
    public Vector2 windForce = new(6f, 0f); // right push; use (0,6) for updraft
    public bool scaleByMass = true;

    void OnTriggerStay2D(Collider2D other){
        if(!other.CompareTag("Player")) return;
        var rb = other.attachedRigidbody;
        if(!rb) return;
        var f = scaleByMass ? windForce * rb.mass : windForce;
        rb.AddForce(f * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }
}
