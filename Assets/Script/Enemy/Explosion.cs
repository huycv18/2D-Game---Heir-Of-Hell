using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            // Truyền vị trí tâm vụ nổ → Player bị đẩy ra ngoài
            player?.TakeDamage(damage, transform.position);
        }

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy?.TakeDamage(damage);
            // Knockback enemy ra ngoài tính từ tâm vụ nổ
            float dir = collision.transform.position.x > transform.position.x ? 1 : -1;
            enemy?.ApplyKnockback(dir, 8f);
        }
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}