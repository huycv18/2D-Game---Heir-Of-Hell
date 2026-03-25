using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Drop Physics")]
    [SerializeField] private float throwForceX = 3f;      // lực văng ngang ngẫu nhiên
    [SerializeField] private float throwForceY = 5f;      // lực tung lên

    [Header("Magnet")]
    [SerializeField] private float pickupDelay = 0.6f;    // delay trước khi bị hút
    [SerializeField] private float magnetRange = 2.5f;    // khoảng cách bắt đầu hút
    [SerializeField] private float magnetSpeed = 8f;      // tốc độ bay về phía player
    [SerializeField] private float collectRange = 0.4f;   // khoảng cách để collect

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform player;
    private bool canBePickedUp = false;
    private bool isBeingMagnetized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Collider là vật lý (KHÔNG phải trigger) để coin đứng trên sàn
        col.isTrigger = false;
    }

    private void Start()
    {
        // Tìm player - tương thích với hệ thống Possession
        PlayerBrain brain = FindAnyObjectByType<PlayerBrain>();
        if (brain != null)
        {
            Transform currentTransform = brain.GetCurrentTransform();
            if (currentTransform != null)
                player = currentTransform;
        }

        // Fallback: tìm PlayerController cũ nếu chưa chuyển sang hệ thống mới
        if (player == null)
        {
            PlayerController pc = FindAnyObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
        }

        // Giữ trục: không xoay, không lăn
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Ignore collision với Player
        if (player != null)
        {
            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null)
                Physics2D.IgnoreCollision(col, playerCol, true);
        }

        // Ignore collision với tất cả Enemy (dùng layer "Enemy")
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int itemLayer  = gameObject.layer;
        if (enemyLayer >= 0 && itemLayer >= 0)
            Physics2D.IgnoreLayerCollision(itemLayer, enemyLayer, true);

        // Ignore collision với tất cả Item khác
        int itemLayerSelf = gameObject.layer;
        if (itemLayerSelf >= 0)
            Physics2D.IgnoreLayerCollision(itemLayerSelf, itemLayerSelf, true);

        // Tung lên với lực ngẫu nhiên
        float randomX = Random.Range(-throwForceX, throwForceX);
        rb.AddForce(new Vector2(randomX, throwForceY), ForceMode2D.Impulse);

        // Bật pickup sau delay
        StartCoroutine(EnablePickupAfterDelay());
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(pickupDelay);
        canBePickedUp = true;
    }

    private void Update()
    {
        if (!canBePickedUp) return;

        // Dynamically update player transform (để theo body hiện tại)
        PlayerBrain brain = FindAnyObjectByType<PlayerBrain>();
        if (brain != null)
        {
            Transform currentTransform = brain.GetCurrentTransform();
            if (currentTransform != null)
                player = currentTransform;
        }

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Vào tầm hút → bắt đầu bay về player
        if (dist <= magnetRange)
            isBeingMagnetized = true;

        if (isBeingMagnetized)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );

            // Đủ gần → collect
            if (dist <= collectRange)
            {
                OnCollected();
            }
        }
    }

    private void OnCollected()
    {
        // Tìm PlayerCollision trên player để gọi collect
        PlayerCollision pc = player.GetComponent<PlayerCollision>();
        if (pc != null)
            pc.CollectItem(gameObject);
    }
}
