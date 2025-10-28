using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    [Header("Ground Move")]
    public float moveSpeed = 3.5f;

    [Header("Jump Charge")]
    public float maxChargeTime = 0.8f;                 // seconds to full charge
    public AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Jump Tuning (height-derived)")]
    public float targetMaxHeight = 3.0f;               // max height at full charge
    public float minHeight = 0.8f;                      // height on quick tap
    public float maxHorizImpulse = 6f;                  // horizontal at full charge
    public float airControl = 0f;   

    private AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip landSound;
    private bool wasGrounded;
                   

    // runtime
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isGrounded = false;
    private bool isCharging = false;
    private bool jumpRequest = false;
    private float chargeTimer = 0f;

    private int facing = 1;             // last seen direction from ground input
    private int chargeDir = 0;          // -1, 0, +1 chosen during charge
    private bool horizDuringCharge = false;

    // impulses
    private float minJumpImpulse;
    private float maxJumpImpulse;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        RecomputeJumpImpulses();
    }

    void OnValidate()
    {
        if (targetMaxHeight < 0.1f) targetMaxHeight = 0.1f;
        if (minHeight < 0.1f) minHeight = 0.1f;
        if (minHeight > targetMaxHeight) minHeight = targetMaxHeight * 0.9f;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        RecomputeJumpImpulses();
    }

    void RecomputeJumpImpulses()
    {
        float g = -Physics2D.gravity.y * (rb ? rb.gravityScale : 1f);
        minJumpImpulse = Mathf.Sqrt(2f * g * minHeight);
        maxJumpImpulse = Mathf.Sqrt(2f * g * targetMaxHeight);
    }

    void Update()
    {

        float xRaw = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVel", rb.velocity.y);

        if (Mathf.Abs(xRaw) > 0.01f) facing = xRaw > 0 ? 1 : -1;

        // Flip sprite visually
        if (sr) sr.flipX = (facing < 0);

        // begin charge only if grounded
        if (!isCharging && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            chargeTimer = 0f;

            // decide initial horizontal intent: ONLY if A/D held now
            if (Mathf.Abs(xRaw) > 0.01f)
            {
                chargeDir = (xRaw > 0 ? 1 : -1);
                horizDuringCharge = true;
            }
            else
            {
                chargeDir = 0;              // vertical-only by default
                horizDuringCharge = false;
            }
        }

        // accumulate charge while holding
        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            chargeTimer = Mathf.Min(chargeTimer + Time.deltaTime, maxChargeTime);

            // allow changing intent during charge: press A/D to enable diagonal
            if (Mathf.Abs(xRaw) > 0.01f)
            {
                chargeDir = (xRaw > 0 ? 1 : -1);
                horizDuringCharge = true;
            }
            // if player releases A/D while charging, we keep last intent (common feel)
        }

        // release → request a single impulse
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            isCharging = false;
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");

        if (isCharging && isGrounded)
        {
            // No creeping while charging
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else if (isGrounded)
        {
            // Normal ground move
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Air control
            float targetX = x * moveSpeed;
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetX, airControl), rb.velocity.y);
        }

        if (jumpRequest)
        {
            // compute charge 
            float t = Mathf.Clamp01(chargeTimer / maxChargeTime);
            float k = chargeCurve.Evaluate(t);

            // vertical impulse from height targets
            float vy = Mathf.Lerp(minJumpImpulse, maxJumpImpulse, k);

            // horizontal ONLY if A/D was held during charge; else vertical jump
            float vx = horizDuringCharge ? Mathf.Lerp(0f, maxHorizImpulse, k) * (chargeDir != 0 ? chargeDir : facing) : 0f;

            // takeoff
            rb.velocity = Vector2.zero;

            // animator jump
            animator.SetBool("IsJumping", true);

            if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);

            // single impulse
            rb.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);

            // reset
            jumpRequest = false;
            horizDuringCharge = false;
            chargeDir = 0;
        }
    }

    // Ground detection
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            OnLanding();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

    // Detect Landing for Animation
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        if (audioSource && landSound) audioSource.PlayOneShot(landSound);
    }

}
