using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public abstract class Enemy : MonoBehaviour
{
    // ── State Machine ────────────────────────────────────────────
    public enum EnemyState { Patrol, Alert, Chase, Attack, Hurt, Dead }
    protected EnemyState currentState = EnemyState.Patrol;

    [Header("Movement")]
    [SerializeField] protected float enemyMoveSpeed = 1.5f;
    [SerializeField] protected float patrolSpeed    = 1f;
    [SerializeField] protected float patrolDistance = 3f;

    [Header("Detection")]
    [SerializeField] protected float alertRange  = 4f;   // phát hiện → Alert
    [SerializeField] protected float chaseRange  = 6f;   // bắt đầu Chase
    [SerializeField] protected float attackRange = 0.8f; // dừng + Attack
    [SerializeField] protected float alertDelay  = 0.5f; // giây dừng lại trước khi Chase

    [Header("Stats")]
    [SerializeField] protected float maxHP      = 50f;
    [SerializeField] protected Image hpBar;
    [SerializeField] protected float enterDamage = 20f;
    [SerializeField] protected float stayDamage  = 5f;
    [SerializeField] protected float knockbackTime = 0.2f;

    [Header("Edge & Wall Detection")]
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private float wallCheckDistance   = 0.3f;
    [SerializeField] private float edgeCheckOffsetX    = 0.4f; // lệch ra phía trước bao nhiêu
    [SerializeField] private float edgeCheckOffsetY    = -0.5f; // thấp hơn pivot bao nhiêu
    [SerializeField] private LayerMask groundLayer;

    private float flipCooldown = 0f; // chống lắc liên tục

    [Header("Drop")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinDropCount = 1;

    // Event báo RoomSpawner khi enemy chết
    public event Action<Enemy> OnDeath;

    protected float       currentHP;
    protected bool        isKnockback;
    protected HitFlash    hitFlash;
    protected AudioManager audioManager;
    protected PlayerController player;
    protected Rigidbody2D rb;

    private Vector3 patrolOrigin;
    private int     patrolDir = 1;
    private bool    alertCoroutineRunning = false;

    // ── Unity Lifecycle ──────────────────────────────────────────
    protected virtual void OnEnable()
    {
        currentHP    = maxHP;
        currentState = EnemyState.Patrol;
        patrolOrigin = transform.position;
        alertCoroutineRunning = false;
        // Random hướng patrol ban đầu → mỗi enemy đi một hướng khác nhau
        patrolDir = UnityEngine.Random.value > 0.5f ? 1 : -1;
        UpdateHpBar();
    }

    protected virtual void Start()
    {
        rb           = GetComponent<Rigidbody2D>();
        hitFlash     = GetComponent<HitFlash>();
        audioManager = FindAnyObjectByType<AudioManager>();
        player       = FindAnyObjectByType<PlayerController>();
        currentHP    = maxHP;
        patrolOrigin = transform.position;
        patrolDir    = UnityEngine.Random.value > 0.5f ? 1 : -1;
        UpdateHpBar();
    }

    protected virtual void Update()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Hurt) return;
        if (player == null) player = FindAnyObjectByType<PlayerController>();

        if (flipCooldown > 0f) flipCooldown -= Time.deltaTime;

        UpdateState();
        HandleState();
    }

    // ── State Machine ────────────────────────────────────────────
    private void UpdateState()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (dist <= alertRange && !alertCoroutineRunning)
                    StartCoroutine(AlertCoroutine());
                break;

            case EnemyState.Chase:
                if (dist <= attackRange)
                    currentState = EnemyState.Attack;
                else if (dist > chaseRange)
                    currentState = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                if (dist > attackRange)
                    currentState = EnemyState.Chase;
                break;
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:  HandlePatrol();  break;
            case EnemyState.Chase:   HandleChase();   break;
            case EnemyState.Attack:  HandleAttack();  break;
        }
    }

    /// <summary>
    /// Kiểm tra phía trước theo hướng dir có thể đi tiếp không.
    /// Trả về false nếu: không có ground (vực) HOẶC có tường (wall).
    /// </summary>
    private bool CanMoveInDirection(float dir)
    {
        // groundLayer chưa set → không check, cho đi bình thường
        if (groundLayer.value == 0) return true;

        Vector2 edgeOrigin = (Vector2)transform.position
                           + new Vector2(dir * edgeCheckOffsetX, edgeCheckOffsetY);

        // 1. Kiểm tra vực — ray xuống dưới
        bool hasGround = Physics2D.Raycast(edgeOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // 2. Kiểm tra tường — ray ngang từ giữa thân
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(0, 0.1f);
        bool hasWall = Physics2D.Raycast(wallOrigin, new Vector2(dir, 0), wallCheckDistance, groundLayer);

        return hasGround && !hasWall;
    }

    private void HandlePatrol()
    {
        float distFromOrigin = transform.position.x - patrolOrigin.x;
        if (distFromOrigin >= patrolDistance)       patrolDir = -1;
        else if (distFromOrigin <= -patrolDistance) patrolDir =  1;

        // Phía trước là vực hoặc tường → đổi chiều (có cooldown chống lắc)
        if (!CanMoveInDirection(patrolDir) && flipCooldown <= 0f)
        {
            patrolDir   *= -1;
            flipCooldown = 0.5f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity    = new Vector2(patrolDir * patrolSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(patrolDir, 1, 1);
    }

    private void HandleChase()
    {
        if (player == null) return;

        float dirX = player.transform.position.x > transform.position.x ? 1f : -1f;

        // Phía trước là vực hoặc tường → dừng lại chờ Player
        if (!CanMoveInDirection(dirX))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity    = new Vector2(dirX * enemyMoveSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(dirX, 1, 1);
    }

    private void HandleAttack()
    {
        // Chỉ dừng trục X, giữ Y cho gravity
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (player != null)
        {
            float dir = player.transform.position.x > transform.position.x ? 1f : -1f;
            transform.localScale = new Vector3(dir, 1, 1);
        }
        PerformAttack();
    }

    /// <summary>Subclass override để thực hiện tấn công riêng.</summary>
    protected virtual void PerformAttack() { }

    private IEnumerator AlertCoroutine()
    {
        alertCoroutineRunning = true;
        currentState = EnemyState.Alert;

        // Dừng lại trong alertDelay giây
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(alertDelay);

        currentState = EnemyState.Chase;
        alertCoroutineRunning = false;
    }

    // ── Damage / Knockback ───────────────────────────────────────
    public virtual void TakeDamage(float damage)
    {
        if (currentState == EnemyState.Dead) return;

        currentHP -= damage;
        currentHP  = Mathf.Max(currentHP, 0);
        UpdateHpBar();
        audioManager?.PlayImpactSound();
        hitFlash?.TakeDamageFlash();
        FloatingTextManager.Instance?.ShowValue(-(int)damage, transform.position);

        // Bị đánh → Chase ngay (bỏ qua Alert)
        if (currentState == EnemyState.Patrol || currentState == EnemyState.Alert)
        {
            StopAllCoroutines();
            alertCoroutineRunning = false;
            currentState = EnemyState.Chase;
        }

        if (currentHP <= 0) Die();
    }

    public void ApplyKnockback(float direction, float force)
    {
        StartCoroutine(KnockbackCoroutine(direction, force));
    }

    private IEnumerator KnockbackCoroutine(float direction, float force)
    {
        currentState = EnemyState.Hurt;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * force, 0), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        if (currentState == EnemyState.Hurt)
            currentState = EnemyState.Chase;
    }

    // ── Die ──────────────────────────────────────────────────────
    protected virtual void Die()
    {
        if (currentState == EnemyState.Dead) return;
        currentState = EnemyState.Dead;

        OnDeath?.Invoke(this);
        DropCoins();
        gameObject.SetActive(false); // RoomSpawner tái sử dụng thay vì Destroy
    }

    private void DropCoins()
    {
        if (coinPrefab == null) return;
        for (int i = 0; i < coinDropCount; i++)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
    }

    protected void UpdateHpBar()
    {
        if (hpBar != null) hpBar.fillAmount = currentHP / maxHP;
    }

    // ── Gizmos ───────────────────────────────────────────────────
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ edge ray + wall ray theo cả 2 hướng
        foreach (float dir in new float[] { 1f, -1f })
        {
            Vector2 edgeOrigin = (Vector2)transform.position
                               + new Vector2(dir * edgeCheckOffsetX, edgeCheckOffsetY);
            // Edge ray (xuống)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * groundCheckDistance);
            Gizmos.DrawWireSphere(edgeOrigin, 0.05f);

            // Wall ray (ngang)
            Vector2 wallOrigin = (Vector2)transform.position + new Vector2(0, 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallOrigin, wallOrigin + new Vector2(dir * wallCheckDistance, 0));
        }
    }
}
