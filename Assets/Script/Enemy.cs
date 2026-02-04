using UnityEngine;
using UnityEngine.UI;

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


    protected PlayerController player;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        UpdateHpBar();
    }

    protected virtual void Update()
    {
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
    if (currentHP <= 0)
    {
        Die();
    }
}

protected virtual void Die()
{
    Destroy(gameObject);
}
protected void UpdateHpBar()
{
    if (hpBar != null)
    {
        hpBar.fillAmount = currentHP / maxHP;
    }
}


}
