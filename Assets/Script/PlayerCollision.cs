using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            PlayerController player = GetComponent<PlayerController>();
            player.TakeDamage(10f);
        }
    }
}