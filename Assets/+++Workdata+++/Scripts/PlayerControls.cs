using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float accelerationTime = 0.5f; // Time to reach full speed
    public AnimationCurve accelerationCurve;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float variableJumpMultiplier = 0.5f;
    public int maxJumps = 1;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpsRemaining;
    private bool isJumping;
    private float moveInput;
    private bool facingRight = true;

    private float moveTime = 0f;
    private bool wasMovingLastFrame = false;

    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        // prevents the player sprite from rotating
        rb.freezeRotation = true;
        jumpsRemaining = maxJumps;

        // Default linear curve if none set in inspector
        if (accelerationCurve == null || accelerationCurve.length == 0)
        {
            accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            if (!wasMovingLastFrame)
                moveTime = 0f; // reset when starting new movement
            else
                moveTime += Time.deltaTime;
        }
        else
        {
            moveTime = 0f;
        }

        wasMovingLastFrame = moveInput != 0;

        // Jump input
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpsRemaining--;
        }

        // Variable jump
        if (Input.GetButtonUp("Jump") && isJumping)
        {
            if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpMultiplier);
            isJumping = false;
        }

        // Sprite flipping
        if (facingRight && moveInput < 0)
            Flip();
        else if (!facingRight && moveInput > 0)
            Flip();
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            isJumping = false;
        }

        // Target speed (walk or sprint)
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Normalize move time to 0–1
        float normalizedTime = Mathf.Clamp01(moveTime / accelerationTime);
        float speedMultiplier = accelerationCurve.Evaluate(normalizedTime);

        float currentSpeed = targetSpeed * speedMultiplier;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}


