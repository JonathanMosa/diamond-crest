using UnityEngine;

public class SlimePatrol_Hardcoded : MonoBehaviour
{
    public Transform rightBound;    // right edge of patrol
    public Transform sprite;        // slime sprite
    public float speed = 1.5f;
    public float patrolDistance = 6f;
    public float knockbackForce = 8f; // how strong the hit is
    
    private AudioSource audioSource;
    public AudioClip hitSound;


    private Rigidbody2D rb;
    private int dir = 1;            
    private float leftX;            
    private float rightX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = transform;

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

        // turn at bounds
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
                // get direction away from slime
                float direction = collision.transform.position.x > transform.position.x ? 1f : -1f;
                Vector2 knock = new Vector2(direction * knockbackForce, knockbackForce / 2f);
                playerRb.AddForce(knock, ForceMode2D.Impulse);
            }
        }
        if (audioSource && hitSound)
            audioSource.PlayOneShot(hitSound);

    }
}
