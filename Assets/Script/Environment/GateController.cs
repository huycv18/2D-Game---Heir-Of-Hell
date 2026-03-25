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

    [Header("Win Gate")]
    [Tooltip("Tích vào nếu đây là Gate kết thúc game → hiện UI Win thay vì load scene")]
    [SerializeField] private bool isWinGate = false;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    [Tooltip("Khoảng cách Player lại gần Gate để phát audio mở cửa")]
    [SerializeField] private float detectRange = 2f;

    private GameManager gameManager;

    private SpriteRenderer spriteRenderer;

    [Header("Colliders")]
    [Tooltip("BoxCollider2D Is Trigger = true  → detect Player vào Gate")]
    [SerializeField] private BoxCollider2D triggerCollider;
    [Tooltip("BoxCollider2D Is Trigger = false → chặn Player đi qua khi đóng")]
    [SerializeField] private BoxCollider2D blockCollider;

    private bool hasUsb = false;
    private bool isOpen = false;
    private bool audioPlayed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        if (closeSprite != null)
            spriteRenderer.sprite = closeSprite;

        if (blockCollider != null)   blockCollider.enabled  = true;
        if (triggerCollider != null) triggerCollider.enabled = true;
    }

    public void OpenGate()
    {
        hasUsb = true;
    }

    private void Update()
    {
        if (!hasUsb || isOpen || audioPlayed) return;

        Collider2D player = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Player"));
        if (player != null)
        {
            audioPlayed = true;
            audioManager?.PlayGateOpenSound();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (isOpen) return;

        if (hasUsb)
        {
            isOpen = true;

            if (blockCollider != null) blockCollider.enabled = false;
            if (openSprite != null)    spriteRenderer.sprite = openSprite;

            if (isWinGate)
                StartCoroutine(ShowWinAfterDelay());
            else
                StartCoroutine(LoadSceneAfterDelay());
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        if (LoadingManager.Instance != null && !string.IsNullOrEmpty(nextSceneName))
        {
            LoadingManager.Instance.LoadScene(nextSceneName);
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[Gate] nextSceneName trống!");
        }
    }

    private IEnumerator ShowWinAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        if (gameManager != null)
            gameManager.WinGame();
        else
            Debug.LogWarning("[GateController] isWinGate = true nhưng không tìm thấy GameManager!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = hasUsb ? Color.green : Color.red;
        if (blockCollider != null)
            Gizmos.DrawWireCube(transform.position + (Vector3)blockCollider.offset, blockCollider.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Vẽ icon W màu vàng để phân biệt Win Gate
        if (isWinGate)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, 0.2f);
        }
    }
}
