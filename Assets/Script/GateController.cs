using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GateController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "2";
    [SerializeField] private float delayBeforeLoad = 3f;

    private SpriteRenderer spriteRenderer;

    [Header("Colliders")]
    [Tooltip("BoxCollider2D Is Trigger = true  → detect Player vào Gate")]
    [SerializeField] private BoxCollider2D triggerCollider;
    [Tooltip("BoxCollider2D Is Trigger = false → chặn Player đi qua khi đóng")]
    [SerializeField] private BoxCollider2D blockCollider;

    private bool hasUsb = false;
    private bool isOpen = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (closeSprite != null)
            spriteRenderer.sprite = closeSprite;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (isOpen) return;

        if (hasUsb)
        {
            isOpen = true;

            if (blockCollider != null) blockCollider.enabled = false;

            if (openSprite != null)
                spriteRenderer.sprite = openSprite;

            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = hasUsb ? Color.green : Color.red;
        if (blockCollider != null)
            Gizmos.DrawWireCube(transform.position + (Vector3)blockCollider.offset, blockCollider.size);
    }
}
