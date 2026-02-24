using UnityEngine;

public class HealEnemy : Enemy
{
    [SerializeField] private float healValue = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerController>()?.TakeDamage(enterDamage, transform.position);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerController>()?.TakeDamage(stayDamage, transform.position);
    }

    protected override void Die()
    {
        player?.Heal(healValue);
        base.Die();
    }
}
