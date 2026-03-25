using UnityEngine;

/// <summary>
/// Linh hồn - body mặc định khi không possess
/// Bay tự do, không có attack
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SoulBody : MonoBehaviour, IBody
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float flySpeed = 6f;

    [Header("Physics")]
    [SerializeField] private bool usePhysics = false; // Soul có thể bay xuyên tường

    private Rigidbody2D rb;
    private BodyState state = BodyState.Available;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Soul không bị ảnh hưởng bởi trọng lực
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Soul có thể đi xuyên tường nếu không dùng physics
        if (!usePhysics)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    public void Move(float moveInput)
    {
        if (state != BodyState.Active) return;

        float moveX = moveInput * moveSpeed;
        float moveY = 0f;

        // Soul có thể bay lên xuống
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveY = flySpeed;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveY = -flySpeed;

        if (usePhysics)
            rb.linearVelocity = new Vector2(moveX, moveY);
        else
            transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime;

        // Flip sprite
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        // Animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", moveInput != 0 || moveY != 0);
        }
    }

    public void Jump()
    {
        // Soul không cần nhảy (bay được rồi)
    }

    public void Attack()
    {
        // Soul không có attack
        Debug.Log("[Soul] Soul cannot attack! Possess a body first.");
    }

    public void OnPossessed()
    {
        state = BodyState.Active;
        gameObject.SetActive(true);
    }

    public void OnReleased()
    {
        state = BodyState.Available;
        rb.linearVelocity = Vector2.zero;
    }

    public Transform GetTransform() => transform;

    public bool CanBePossessed() => false; // Soul không thể bị possess

    public BodyState GetBodyState() => state;
}
