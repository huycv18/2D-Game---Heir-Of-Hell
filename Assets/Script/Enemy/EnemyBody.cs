using UnityEngine;

/// <summary>
/// Enemy có thể bị possess
/// Khi chưa possess: chạy AI
/// Khi bị possess: disable AI, nhận input từ PlayerBrain
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BodyAnimationController))]
public class EnemyBody : MonoBehaviour, IBody
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("AI Settings")]
    [SerializeField] private bool hasAI = true;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRange = 3f;

    [Header("Body Settings")]
    [SerializeField] private bool canBePossessed = true;

    [Header("Tags (Info Only)")]
    [SerializeField] private string currentTag; // Chỉ để xem

    private Rigidbody2D rb;
    private BodyAnimationController animController;
    private BodyState state = BodyState.AIControlled;
    private bool isGrounded;

    // AI variables
    private Vector3 startPosition;
    private float aiDirection = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<BodyAnimationController>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Hiển thị tag hiện tại (không cần đổi)
        currentTag = gameObject.tag;
        Debug.Log($"[EnemyBody] {gameObject.name} - Tag: {currentTag} - Possessable: {canBePossessed}");

        startPosition = transform.position;

        // Tạo ground check nếu chưa có
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.parent = transform;
            gc.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = gc.transform;
        }
    }

    private void Update()
    {
        CheckGround();

        // Chạy AI nếu đang trong trạng thái AIControlled
        if (state == BodyState.AIControlled && hasAI)
        {
            RunAI();
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void RunAI()
    {
        // AI patrol đơn giản
        float distanceFromStart = transform.position.x - startPosition.x;

        if (Mathf.Abs(distanceFromStart) >= patrolRange)
        {
            aiDirection *= -1f;
        }

        rb.linearVelocity = new Vector2(aiDirection * patrolSpeed, rb.linearVelocity.y);

        // Flip sprite
        transform.localScale = new Vector3(
            Mathf.Sign(aiDirection) * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );

        animController?.PlayAnimation(AnimationType.Move);
    }

    public void Move(float moveInput)
    {
        if (state != BodyState.Active) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        animController?.PlayAnimation(Mathf.Abs(moveInput) > 0.1f ? AnimationType.Move : AnimationType.Idle);
    }

    public void Jump()
    {
        if (state != BodyState.Active || !isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animController?.PlayAnimation(AnimationType.Jump);
    }

    public void Attack()
    {
        if (state != BodyState.Active) return;

        animController?.PlayAnimation(AnimationType.Attack);
        Debug.Log($"[EnemyBody] {gameObject.name} attacks!");
    }

    public void OnPossessed()
    {
        state = BodyState.Active;
        gameObject.SetActive(true);
        rb.simulated = true;
        Debug.Log($"[EnemyBody] {gameObject.name} possessed - AI disabled");
    }

    public void OnReleased()
    {
        state = BodyState.Inactive;
        rb.linearVelocity = Vector2.zero;
        animController?.PlayAnimation(AnimationType.Down);
        Debug.Log($"[EnemyBody] {gameObject.name} released");

        // Có thể enable lại AI sau một khoảng thời gian
        // Invoke(nameof(EnableAI), 2f);
    }

    private void EnableAI()
    {
        if (state == BodyState.Inactive)
        {
            state = BodyState.AIControlled;
            Debug.Log($"[EnemyBody] {gameObject.name} AI re-enabled");
        }
    }

    public Transform GetTransform() => transform;

    public bool CanBePossessed() => canBePossessed && (state == BodyState.AIControlled || state == BodyState.Inactive);

    public BodyState GetBodyState() => state;

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Vẽ patrol range
        if (hasAI && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPosition + Vector3.left * patrolRange, startPosition + Vector3.right * patrolRange);
        }
    }
}
