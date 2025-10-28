using UnityEngine;

public class SkeletonPatrol_Hardcoded : MonoBehaviour
{
    public Transform rightBound;    // right edge of patrol
    public Transform sprite;        // skeleton sprite
    public float speed = 1.2f;
    public float patrolDistance = 3f; // 3 tiles
    public float knockbackForce = 7f; // how strong the hit is

    private AudioSource audioSource;
    public AudioClip hitSound;

    private Rigidbody2D rb;
    private int dir = 1;
    private float leftX;
    private float rightX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = transform;
        audioSource = GetComponent<AudioSource>();

        rightX = rightBound.position.x;
        leftX = rightX - patrolDistance;
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

        // flip direction at patrol bounds
        if (transform.position.x >= rightX && dir > 0)
            dir = -1;
        else if (transform.position.x <= leftX && dir < 0)
            dir = 1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // direction away from skeleton
                float direction = collision.transform.position.x > transform.position.x ? 1f : -1f;
                Vector2 knock = new Vector2(direction * knockbackForce, knockbackForce / 2f);
                playerRb.AddForce(knock, ForceMode2D.Impulse);
            }

            if (audioSource && hitSound)
                audioSource.PlayOneShot(hitSound);
        }
    }
}
