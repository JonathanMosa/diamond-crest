using UnityEngine;

public class SlimePatrol_Hardcoded : MonoBehaviour
{
    public Transform rightBound;    // assign the right edge object
    public Transform sprite;        // assign your slime sprite child
    public float speed = 1.5f;
    public float patrolDistance = 6f; // total width in tiles (6 units if 1 tile = 1 unit)

    private Rigidbody2D rb;
    private int dir = 1;            // 1 = right, -1 = left
    private float leftX;            // computed from rightBound
    private float rightX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = transform;

        // left and right computation
        rightX = rightBound.position.x;
        leftX = rightX - patrolDistance;  // 6 tiles to the left
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        // flip sprite
        if (dir != 0)
            sprite.localScale = new Vector3(
                Mathf.Abs(sprite.localScale.x) * (dir > 0 ? 1 : -1),
                sprite.localScale.y,
                sprite.localScale.z
            );

        // flip direction at bounds
        if (transform.position.x >= rightX && dir > 0)
            dir = -1;
        else if (transform.position.x <= leftX && dir < 0)
            dir = 1;
    }
}
