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
    }

    // Gọi từ PlayerCollision.OnTriggerStay2D
    public void StayDamagePlayer(PlayerController player)
    {
        if (player == null || iFrameTimer > 0f) return;

        stayTimer += Time.deltaTime;
        if (stayTimer >= stayDamageInterval)
        {
            stayTimer = 0f;
            player.TakeDamage(stayDamagePerSec * stayDamageInterval, transform.position);
            iFrameTimer = iFrameAfterEnter;
        }
    }

    // Gọi từ PlayerCollision.OnTriggerExit2D
    public void ResetStayTimer()
    {
        stayTimer = 0f;
    }
}
