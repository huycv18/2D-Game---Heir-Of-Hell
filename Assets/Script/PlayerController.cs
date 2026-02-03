using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxJumpCount = 2;
    private int jumpCount;

    private bool isLanding;
    private bool isFalling;


    private bool isGrounded;
    private Animator animator;

    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimation();
        HandleLanding();
        HandleFalling(); 
    }

    private void HandleMovement()
{
    float moveInput = Input.GetAxis("Horizontal");
    rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
    else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
}
private void HandleJump()
{
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);

    if (isGrounded)
    {
        jumpCount = 0;
    }

    if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;
    }
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



}
