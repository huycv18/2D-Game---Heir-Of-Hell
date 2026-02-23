using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "2";

    private SpriteRenderer spriteRenderer;

    [Header("Colliders")]
    [Tooltip("BoxCollider2D Is Trigger = true  → detect Player vào Gate")]
    [SerializeField] private BoxCollider2D triggerCollider;
    [Tooltip("BoxCollider2D Is Trigger = false → chặn Player đi qua khi đóng")]
    [SerializeField] private BoxCollider2D blockCollider;

    private bool hasUsb = false;  // Player đã nhặt Usb chưa
    private bool isOpen = false;  // Gate đã mở (đang load scene) chưa

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (closeSprite != null)
            spriteRenderer.sprite = closeSprite;

        // Khi bắt đầu: chặn vật lý BẬT, trigger BẮT
        if (blockCollider != null) blockCollider.enabled = true;
        if (triggerCollider != null) triggerCollider.enabled = true;
    }

    /// <summary>
    /// Được gọi từ PlayerCollision khi Player nhặt Usb.
    /// Chỉ lưu trạng thái, Gate chưa mở ngay.
    /// </summary>
    public void OpenGate()
    {
        hasUsb = true;
        Debug.Log("Usb đã nhặt! Tiến lại gần Gate để mở cửa.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (isOpen) return;

        if (hasUsb)
        {
            // Player tiến vào Gate và có Usb → tắt chặn, mở Gate, chuyển scene
            isOpen = true;

            if (blockCollider != null) blockCollider.enabled = false;

            if (openSprite != null)
                spriteRenderer.sprite = openSprite;

            Debug.Log("Gate mở! Chuyển sang màn tiếp theo...");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // Hiển thị vùng trigger trong Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = hasUsb ? Color.green : Color.red;
        if (blockCollider != null)
            Gizmos.DrawWireCube(transform.position + (Vector3)blockCollider.offset, blockCollider.size);
    }
}
