using UnityEngine;

public class SkeletonPatrol_Hardcoded : MonoBehaviour
{
    public Transform rightBound;    // place this at the right edge
    public Transform sprite;        // assign the skeleton sprite child
    public float speed = 1.2f;
    public float patrolDistance = 3f; // 3 tiles

    Rigidbody2D rb;
    int dir = 1;                    // 1 = right, -1 = left
    float leftX, rightX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!sprite) sprite = transform;

        rightX = rightBound.position.x;
        leftX  = rightX - patrolDistance; // 3 tiles left of rightBound
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        // face move direction
        var s = sprite.localScale;
        sprite.localScale = new Vector3(Mathf.Abs(s.x) * (dir > 0 ? 1 : -1), s.y, s.z);

        // flip at ends
        if (dir > 0 && transform.position.x >= rightX) dir = -1;
        else if (dir < 0 && transform.position.x <= leftX) dir = 1;
    }
}
