using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float enemyMoveSpeed = 1.5f;
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float maxHP = 50f;
    protected float currentHP;
    [SerializeField] protected Image hpBar;
    [SerializeField] protected float enterDamage = 20f;
    [SerializeField] protected float stayDamage = 5f;
    [SerializeField] protected float knockbackTime = 0.2f;

    [Header("Drop")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinDropCount = 1;

protected bool isKnockback;
protected HitFlash hitFlash;

    protected AudioManager audioManager;
    protected PlayerController player;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindAnyObjectByType<AudioManager>();
        currentHP = maxHP;
        UpdateHpBar();
        hitFlash = GetComponent<HitFlash>();
    }

    protected virtual void Update()
{
    if (isKnockback) return;

    HandleMovement();
}

    protected void HandleMovement()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        // Player ngoài tầm → đứng yên
        if (distance > chaseRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Player trong tầm → chỉ đi ngang
        MoveHorizontal();
        FlipEnemy();
    }

    protected void MoveHorizontal()
    {
        float direction = player.transform.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * enemyMoveSpeed, rb.linearVelocity.y);
    }

    protected void FlipEnemy()
    {
        float dir = player.transform.position.x < transform.position.x ? -1 : 1;
        transform.localScale = new Vector3(dir, 1, 1);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHpBar();
        audioManager?.PlayImpactSound();
        hitFlash?.TakeDamageFlash();
        FloatingTextManager.Instance?.ShowValue(-(int)damage, transform.position);
        if (currentHP <= 0)
            Die();
    }

protected virtual void Die()
{
    DropCoins();
    Destroy(gameObject);
}

private void DropCoins()
{
    if (coinPrefab == null) return;

    for (int i = 0; i < coinDropCount; i++)
    {
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
    }
}
protected void UpdateHpBar()
{
    if (hpBar != null)
    {
        hpBar.fillAmount = currentHP / maxHP;
    }
}
public void ApplyKnockback(float direction, float force)
{
    StartCoroutine(KnockbackCoroutine(direction, force));
}

private System.Collections.IEnumerator KnockbackCoroutine(float direction, float force)
{
    isKnockback = true;

    rb.linearVelocity = Vector2.zero;
    rb.AddForce(new Vector2(direction * force, 0), ForceMode2D.Impulse);

    yield return new WaitForSeconds(knockbackTime);

    isKnockback = false;
}
}
