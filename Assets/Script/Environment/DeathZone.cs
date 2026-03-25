using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void Kill(Collider2D collision)
    {
        // Player
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(99999f);
            return;
        }

        // Enemy — dùng GetComponent để bắt tất cả loại enemy bất kể tag
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(99999f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)  => Kill(collision);
    private void OnTriggerStay2D(Collider2D collision)   => Kill(collision);
    private void OnCollisionEnter2D(Collision2D collision) => Kill(collision.collider);
    private void OnCollisionStay2D(Collision2D collision)  => Kill(collision.collider);
}
