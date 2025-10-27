using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BubbleOrb : MonoBehaviour
{
    public float bounceMultiplier = 1.1f;   // energy gain
    public float minHoriz = 4f;             // ensure a real lateral launch

    void OnTriggerEnter2D(Collider2D other){
        if(!other.CompareTag("Player")) return;
        var rb = other.attachedRigidbody;
        if(!rb) return;

        var v = rb.velocity;
        // Flip horizontal direction; preserve vertical with slight boost
        float newX = -Mathf.Sign(v.x) * Mathf.Max(Mathf.Abs(v.x)*bounceMultiplier, minHoriz);
        float newY = Mathf.Max(v.y * bounceMultiplier, 6f);
        rb.velocity = Vector2.zero; // clean reflect
        rb.AddForce(new Vector2(newX, newY), ForceMode2D.Impulse);
    }
}
