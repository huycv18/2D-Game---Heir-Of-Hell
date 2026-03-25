using UnityEngine;

/// <summary>
/// Body của Player - thân xác cơ bản với khả năng cân bằng
/// Refactored từ PlayerController để implement IBody
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BodyAnimationController))]
public class PlayerBody : MonoBehaviour, IBody
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Body Settings")]
    [SerializeField] private BodyState initialState = BodyState.Available;
    [SerializeField] private bool isPossessable = true;

    [Header("Animation")]
    [SerializeField] private bool useAnimation = true; // Tắt nếu chưa setup animation

    [Header("Tags (Info Only)")]
    [SerializeField] private string currentTag; // Chỉ để xem, không cần đổi

    private Rigidbody2D rb;
    private BodyAnimationController animController;
    private BodyState state;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<BodyAnimationController>();
        state = initialState;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Hiển thị tag hiện tại (không cần đổi)
        currentTag = gameObject.tag;
        Debug.Log($"[PlayerBody] {gameObject.name} - Tag: {currentTag} - Possessable: {isPossessable}");

        // Kiểm tra animation setup
        if (useAnimation)
        {
            if (animController == null)
            {
                Debug.LogWarning($"[PlayerBody] {gameObject.name} - useAnimation = true nhưng không có BodyAnimationController!");
                Debug.LogWarning($"[PlayerBody] Bạn có thể: 1) Add BodyAnimationController, hoặc 2) Tắt useAnimation");
                useAnimation = false;
            }
            else if (GetComponent<Animator>() == null)
            {
                Debug.LogWarning($"[PlayerBody] {gameObject.name} - Không có Animator component! Tắt animation.");
                useAnimation = false;
            }
        }

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
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void Move(float moveInput)
    {
        if (state != BodyState.Active)
        {
            if (moveInput != 0) // Chỉ log khi có input
                Debug.LogWarning($"[PlayerBody] Cannot move - State is {state}, not Active!");
            return;
        }

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

        // Animation (chỉ khi bật)
        if (useAnimation && animController != null)
        {
            animController.PlayAnimation(Mathf.Abs(moveInput) > 0.1f ? AnimationType.Move : AnimationType.Idle);
        }
    }

    public void Jump()
    {
        if (state != BodyState.Active || !isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (useAnimation && animController != null)
            animController.PlayAnimation(AnimationType.Jump);
    }

    public void Attack()
    {
        if (state != BodyState.Active) return;

        if (useAnimation && animController != null)
            animController.PlayAnimation(AnimationType.Attack);

        // TODO: Implement attack logic
        Debug.Log("[PlayerBody] Attack!");
    }

    public void OnPossessed()
    {
        state = BodyState.Active;
        gameObject.SetActive(true);
        rb.simulated = true;
        Debug.Log($"[PlayerBody] {gameObject.name} possessed");
    }

    public void OnReleased()
    {
        state = BodyState.Inactive;
        rb.linearVelocity = Vector2.zero;

        if (useAnimation && animController != null)
            animController.PlayAnimation(AnimationType.Down);

        Debug.Log($"[PlayerBody] {gameObject.name} released");
    }

    public Transform GetTransform() => transform;

    public bool CanBePossessed()
    {
        // Kiểm tra state và flag isPossessable
        bool canPossess = isPossessable && (state == BodyState.Available || state == BodyState.Inactive);
        return canPossess;
    }

    public BodyState GetBodyState() => state;

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
