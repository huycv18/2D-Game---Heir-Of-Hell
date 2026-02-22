using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        Enemy enemy = collision.GetComponent<Enemy>();

        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }

        if (collision.CompareTag("Enemy"))
        {
            enemy.TakeDamage(damage);
        }
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}