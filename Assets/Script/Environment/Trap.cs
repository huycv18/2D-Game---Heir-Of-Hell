using UnityEngine;

/// <summary>
/// Trap gây sát thương cho Player.
/// Logic collision được xử lý bởi PlayerCollision.cs (phía Player).
/// </summary>
public class Trap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float enterDamage = 15f;
    [SerializeField] private float stayDamagePerSec = 8f;
    [SerializeField] private float stayDamageInterval = 0.5f;

    [Header("Invincibility Frame")]
    [SerializeField] private float iFrameAfterEnter = 0.6f;

    private float stayTimer = 0f;
    private float iFrameTimer = 0f;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError($"[Trap] ✗✗ KHÔNG CÓ Collider2D trên '{gameObject.name}'!");
        else if (!col.isTrigger)
            Debug.LogWarning($"[Trap] ⚠ Collider2D trên '{gameObject.name}' chưa bật Is Trigger!");
        else
            Debug.Log($"[Trap] ✓ Setup OK — {col.GetType().Name}, IsTrigger={col.isTrigger}");
    }

    private void Update()
    {
        if (iFrameTimer > 0f)
            iFrameTimer -= Time.deltaTime;
    }

    // Gọi từ PlayerCollision.OnTriggerEnter2D
    public void DamagePlayer(PlayerController player)
    {
        if (player == null || iFrameTimer > 0f) return;

        player.TakeDamage(enterDamage, transform.position);
        iFrameTimer = iFrameAfterEnter;
        stayTimer = 0f;
        Debug.Log($"[Trap] ✓ Enter damage {enterDamage} → Player");
    }

    // Gọi từ PlayerCollision.OnTriggerStay2D
    public void StayDamagePlayer(PlayerController player)
    {
        if (player == null || iFrameTimer > 0f) return;

        stayTimer += Time.deltaTime;
        if (stayTimer >= stayDamageInterval)
        {
            stayTimer = 0f;
            float dmg = stayDamagePerSec * stayDamageInterval;
            player.TakeDamage(dmg, transform.position);
            iFrameTimer = iFrameAfterEnter;
            Debug.Log($"[Trap] ✓ Stay damage {dmg} → Player");
        }
    }

    // Gọi từ PlayerCollision.OnTriggerExit2D
    public void ResetStayTimer()
    {
        stayTimer = 0f;
    }
}
