using System.Collections;
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
    [SerializeField] private float attackRangeMultiplier = 1.5f; // Hệ số nhân scope của hitbox (chỉnh lại trong Inspector nếu muốn)
    public GameObject attackHitbox;

    [Header("Knockback")]
    [SerializeField] private float knockbackForceX = 8f;
    [SerializeField] private float knockbackForceY = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Invincibility Frame")]
    [SerializeField] private float iFrameDuration = 0.8f;
    private bool isInvincible = false;
    private float iFrameTimer = 0f;

    private float currentHP;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector3 originalHitboxScale;
    private float attackLockedScale = 1f; // hướng nhân vật bị lock khi đang attack

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool wasGrounded;
    private int jumpCount;

    private bool isLanding;
    private bool isFalling;
    private bool isDead = false;

    private HitFlash hitFlash;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hitFlash = GetComponent<HitFlash>();
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
    }

    void Start()
    {
        currentHP = maxHP;
        UpdateHpBar();
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            originalHitboxScale = attackHitbox.transform.localScale;
        }
    }

    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGameMenu();
        }

        // Đếm thời gian knockback
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false;
        }

        // Đếm iFrame
        if (isInvincible)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer <= 0f)
                isInvincible = false;
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
        // Không cho di chuyển khi đang bị knockback
        if (isKnockedBack) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Đang attack → lock hướng, không cho flip sprite
        if (isAttacking)
        {
            transform.localScale = new Vector3(attackLockedScale, 1, 1);
            return;
        }

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

            // Ghi lại hướng tại thời điểm bắt đầu attack → lock suốt animation
            attackLockedScale = transform.localScale.x;

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
        {
            // Cập nhật độ dài của hitbox (tiện cho việc test đổi giá trị liên tục trong Editor)
            attackHitbox.transform.localScale = new Vector3(originalHitboxScale.x * attackRangeMultiplier, originalHitboxScale.y, originalHitboxScale.z);
            attackHitbox.SetActive(true);
        }
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

        Debug.Log("[PlayerController] Player đã CHẾT! Đang hiển thị GameOverMenu...");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        
        if (gameManager != null)
        {
            gameManager.GameOverMenu();
        }
        else
        {
            Debug.LogError("[PlayerController] ✗ LỖI: GameManager reference là NULL trên Player! Không thể mở GameOverMenu.");
            // Fallback: Tìm GameManager trong scene nếu bị thiếu reference
            GameManager foundGM = Object.FindAnyObjectByType<GameManager>();
            if (foundGM != null) foundGM.GameOverMenu();
        }

        // Delay xóa object hoặc để người chơi nhìn thấy death animation
        Invoke(nameof(DestroyPlayer), 1.2f);
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        if (isDead) return;
        if (isInvincible) return;   // đang trong iFrame → bỏ qua damage

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHpBar();
        audioManager?.PlayImpactSound();
        hitFlash?.TakeDamageFlash();
        FloatingTextManager.Instance?.ShowValue(-(int)damage, transform.position);

        // Bật iFrame
        isInvincible = true;
        iFrameTimer = iFrameDuration;

        // Tính hướng bật ra (ngược chiều với nguồn gây sát thương)
        Vector2 knockbackDir = ((Vector2)transform.position - damageSourcePosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDir.x * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        if (currentHP <= 0)
            Die();
    }

    // Overload để tương thích với code cũ không truyền vị trí
    public void TakeDamage(float damage)
    {
        TakeDamage(damage, transform.position + Vector3.left);
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
            FloatingTextManager.Instance?.ShowValue((int)healValue, transform.position);
        }
    }
}
