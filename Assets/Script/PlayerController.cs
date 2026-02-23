using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxJumpCount = 1;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Effect")]
    [SerializeField] private GameObject smokeJump;

    [Header("Attack")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private Image hpBar;
    [SerializeField] private float attackCooldown = 0.4f;  // Khớp với độ dài animation Attack
    public GameObject attackHitbox;
    private float currentHP;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool wasGrounded;
    private int jumpCount;

    private bool isLanding;
    private bool isFalling;
    private bool isDead = false;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
    }

    void Start()
    {
        currentHP = maxHP;
        UpdateHpBar();
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGameMenu();
        }
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
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
            jumpCount = 0;

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            SpawnSmoke();
            audioManager?.PlayPlayerJumpSound();
        }
    }

    private void CheckLandingSmoke()
    {
        if (!wasGrounded && isGrounded)
            SpawnSmoke();
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
        // Đếm cooldown
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                // Đảm bảo hitbox tắt sau khi cooldown xong
                if (attackHitbox != null)
                    attackHitbox.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            attackTimer = attackCooldown;

            // Reset trigger cũ trước khi set mới → tránh animation bị xếp hàng
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");

            audioManager?.PlayPlayerCombatSound();
        }
    }

    // Gọi từ Animation Event: frame bắt đầu hitbox
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    // Gọi từ Animation Event: frame kết thúc hitbox
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        Invoke(nameof(DestroyPlayer), 1.2f);
        gameManager.GameOverMenu();
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHpBar();
        if (currentHP <= 0)
            Die();
    }

    private void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = currentHP / maxHP;
    }

    public void Heal(float healValue)
    {
        if (currentHP < maxHP)
        {
            currentHP += healValue;
            currentHP = Mathf.Min(currentHP, maxHP);
            UpdateHpBar();
        }
    }
}
