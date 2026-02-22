using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 40f;

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            float dir = collision.transform.position.x > transform.position.x ? 1 : -1;

            enemy.ApplyKnockback(dir, 8f);
        }
    }
}
}