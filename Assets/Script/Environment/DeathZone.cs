using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void KillPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(99999f);
        }
    }

    // Trường hợp Composite Collider Is Trigger = true
    private void OnTriggerEnter2D(Collider2D collision) => KillPlayer(collision);
    private void OnTriggerStay2D(Collider2D collision) => KillPlayer(collision);

    // Trường hợp Composite Collider Is Trigger = false
    private void OnCollisionEnter2D(Collision2D collision) => KillPlayer(collision.collider);
    private void OnCollisionStay2D(Collision2D collision) => KillPlayer(collision.collider);
}
