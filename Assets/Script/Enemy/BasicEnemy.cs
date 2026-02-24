using UnityEngine;

public class BasicEnemy : Enemy
{
    // Attack được xử lý qua Trigger — PerformAttack không cần override
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
}
