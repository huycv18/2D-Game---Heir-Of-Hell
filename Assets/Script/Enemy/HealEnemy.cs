using UnityEngine;

public class HealEnemy : Enemy
{
    [SerializeField] private float healValue = 10f;
    [SerializeField] private float stayDamageInterval = 0.5f;
    private float stayTimer = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stayTimer = 0f;
            collision.GetComponent<PlayerController>()?.TakeDamage(enterDamage, transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayDamageInterval)
            {
                stayTimer = 0f;
                collision.GetComponent<PlayerController>()?.TakeDamage(stayDamage, transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            stayTimer = 0f;
    }

    protected override void Die()
    {
        player?.Heal(healValue);
        base.Die();
    }
}
