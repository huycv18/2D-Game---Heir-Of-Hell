using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GateController gate;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
        // Không tìm Gate ở đây - chỉ tìm khi cần (lúc nhặt USB)
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
            // Tìm Gate đúng lúc cần dùng
            if (gate == null)
                gate = FindAnyObjectByType<GateController>();

            if (gate != null)
                gate.OpenGate();

            audioManager.PlayCoinSound();
            Destroy(item);
        }
    }

    private Trap GetTrap(Collider2D collision)
    {
        // Tìm cả trên chính object, parent và children để không miss cấu trúc phân cấp
        return collision.GetComponent<Trap>()
            ?? collision.GetComponentInParent<Trap>()
            ?? collision.GetComponentInChildren<Trap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            PlayerController player = GetComponent<PlayerController>();
            player.TakeDamage(10f, collision.transform.position);
        }
        else if (collision.CompareTag("Trap"))
        {
            Debug.Log($"[PlayerCollision] OnTriggerEnter2D ← Trap: '{collision.name}'");
            Trap trap = GetTrap(collision);
            if (trap != null)
                trap.DamagePlayer(GetComponent<PlayerController>());
            else
                Debug.LogError($"[PlayerCollision] ✗✗ KHÔNG TÌM THẤY Trap script trên '{collision.name}' và cả parent/children! Hãy gắn script Trap.cs vào GameObject.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            Trap trap = GetTrap(collision);
            if (trap != null)
                trap.StayDamagePlayer(GetComponent<PlayerController>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            Trap trap = GetTrap(collision);
            if (trap != null)
                trap.ResetStayTimer();
            Debug.Log("[PlayerCollision] Player rời Trap");
        }
    }
}