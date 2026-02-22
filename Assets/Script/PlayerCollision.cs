using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
    }

    // Được gọi bởi ItemPickup khi coin/item bay đến đủ gần player
    public void CollectItem(GameObject item)
    {
        if (item == null) return;

        if (item.CompareTag("Coin"))
        {
            gameManager.AddScore(1);
            audioManager.PlayCoinSound();
            Destroy(item);
        }
        else if (item.CompareTag("Energy"))
        {
            gameManager.AddEnergy();
            audioManager.PlayCoinSound();
            Destroy(item);
        }
        else if (item.CompareTag("Usb"))
        {
            Debug.Log("Win Game Roi!!!");
            Destroy(item);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            PlayerController player = GetComponent<PlayerController>();
            player.TakeDamage(10f);
        }
    }
}