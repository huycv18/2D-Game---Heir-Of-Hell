using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Effect")]
    [SerializeField] private GameObject smokeJump;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool wasGrounded;
    private int jumpCount;

    private bool isLanding;
    private bool isFalling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        CheckLandingSmoke();
        UpdateAnimation();
        HandleLanding();
        HandleFalling();
        HandleAttack();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.3f,
            groundLayer
        );
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;

            SpawnSmoke(); 
        }
    }

    private void CheckLandingSmoke()
    {
        if (!wasGrounded && isGrounded)
        {
            SpawnSmoke(); 
        }

        wasGrounded = isGrounded;
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleLanding()
    {
        isLanding = Input.GetKey(KeyCode.S) && isGrounded;
        animator.SetBool("IsLanding", isLanding);
    }

    private void HandleFalling()
    {
        isFalling = !isGrounded && rb.linearVelocity.y < 0;
        animator.SetBool("IsFalling", isFalling);
    }

    private void SpawnSmoke()
    {
        if (smokeJump == null) return;

        GameObject smoke = Instantiate(
            smokeJump,
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            Quaternion.identity
        );

        Destroy(smoke, 0.6f);
    }
    private void HandleAttack()
{
    if (Input.GetMouseButtonDown(0))
    {
        animator.SetTrigger("Attack");
    }
}
public void TakeDamage()
{
    Die();
}

private void Die()
{
    Destroy(gameObject);
}
public GameObject attackHitbox;

public void EnableAttackHitbox()
{
    attackHitbox.SetActive(true);
}

public void DisableAttackHitbox()
{
    attackHitbox.SetActive(false);
}



}
