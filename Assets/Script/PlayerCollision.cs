using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            gameManager.AddScore(1);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Energy"))
        {
            gameManager.AddEnergy();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("EnemyBullet"))
        {
            PlayerController player = GetComponent<PlayerController>();
            player.TakeDamage(10f);
        }
        else if (collision.CompareTag("Usb"))
        {
            Debug.Log("Win Game Roi!!!");
            Destroy(collision.gameObject);
        }
    }
}