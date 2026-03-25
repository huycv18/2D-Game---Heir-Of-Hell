using UnityEngine;

/// <summary>
/// Bộ não của Player - nhận input và điều khiển body hiện tại
/// Đây là thành phần tồn tại xuyên suốt game, không bị destroy
/// </summary>
public class PlayerBrain : MonoBehaviour
{
    [Header("Current Control")]
    [SerializeField] private GameObject initialBody; // Body khởi đầu

    private IBody currentBody;
    private PossessionSystem possessionSystem;

    [Header("Input Settings")]
    [SerializeField] private KeyCode possessKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode releaseKey = KeyCode.Alpha2;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private void Awake()
    {
        possessionSystem = GetComponent<PossessionSystem>();
        if (possessionSystem == null)
            possessionSystem = gameObject.AddComponent<PossessionSystem>();
    }

    private void Start()
    {
        Debug.Log("[PlayerBrain] Starting initialization...");

        // Possess body khởi đầu nếu có
        if (initialBody != null)
        {
            // Disable PlayerController cũ nếu có
            MonoBehaviour oldController = initialBody.GetComponent("PlayerController") as MonoBehaviour;
            if (oldController != null && oldController.enabled)
            {
                oldController.enabled = false;
                Debug.LogWarning($"[PlayerBrain] Auto-disabled old PlayerController on {initialBody.name}");
            }

            IBody body = initialBody.GetComponent<IBody>();
            if (body != null)
            {
                possessionSystem.Possess(body);
                currentBody = body;
                Debug.Log($"✅ [PlayerBrain] Initial body possessed: {initialBody.name}");
                Debug.Log($"🎮 [PlayerBrain] Bạn có thể điều khiển {initialBody.name} bằng WASD/Space!");
            }
            else
            {
                Debug.LogError($"╔════════════════════════════════════════════════════════════╗");
                Debug.LogError($"║  LỖI: GameObject '{initialBody.name}' CHƯA CÓ COMPONENT!  ║");
                Debug.LogError($"╠════════════════════════════════════════════════════════════╣");
                Debug.LogError($"║  CÁCH SỬA:                                                 ║");
                Debug.LogError($"║  1. Chọn GameObject '{initialBody.name}' trong Hierarchy   ║");
                Debug.LogError($"║  2. Inspector → Add Component                              ║");
                Debug.LogError($"║  3. Tìm và add: PlayerBody                                 ║");
                Debug.LogError($"║  4. Add thêm: BodyAnimationController                      ║");
                Debug.LogError($"║  5. Nhấn Play lại                                          ║");
                Debug.LogError($"╚════════════════════════════════════════════════════════════╝");

                // Liệt kê các component hiện có
                Component[] components = initialBody.GetComponents<Component>();
                Debug.LogWarning($"[PlayerBrain] Components hiện có trên '{initialBody.name}':");
                foreach (Component comp in components)
                {
                    Debug.LogWarning($"  - {comp.GetType().Name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[PlayerBrain] Không có initial body! Cần gán trong Inspector.");
        }

        // Kiểm tra currentBody
        if (currentBody == null)
        {
            Debug.LogError("[PlayerBrain] CurrentBody is NULL! Possession sẽ không hoạt động.");
            Debug.LogError("[PlayerBrain] Vui lòng sửa lỗi ở trên và chạy lại game!");
        }
    }

    private void Update()
    {
        if (currentBody == null) return;

        HandleMovementInput();
        HandleActionInput();
        HandlePossessionInput();
    }

    private void HandleMovementInput()
    {
        // Di chuyển
        float moveInput = Input.GetAxis("Horizontal");
        currentBody.Move(moveInput);

        // Nhảy
        if (Input.GetButtonDown("Jump"))
        {
            currentBody.Jump();
        }
    }

    private void HandleActionInput()
    {
        // Tấn công
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            currentBody.Attack();
        }
    }

    private void HandlePossessionInput()
    {
        // Possess body mới
        if (Input.GetKeyDown(possessKey))
        {
            Debug.Log($"[PlayerBrain] Phím [{possessKey}] được nhấn!");

            if (currentBody == null)
            {
                Debug.LogError("[PlayerBrain] CurrentBody is NULL! Không thể possess.");
                return;
            }

            Vector3 searchPosition = currentBody.GetTransform().position;
            Debug.Log($"[PlayerBrain] Tìm kiếm body từ vị trí: {searchPosition}");

            IBody nearbyBody = possessionSystem.FindNearestPossessableBody(searchPosition);

            if (nearbyBody != null && nearbyBody != currentBody)
            {
                Debug.Log($"[PlayerBrain] Tìm thấy body: {nearbyBody.GetTransform().name}");
                Possess(nearbyBody);
            }
            else if (nearbyBody == null)
            {
                Debug.LogWarning("[PlayerBrain] Không tìm thấy body nào trong tầm!");
            }
            else
            {
                Debug.LogWarning("[PlayerBrain] Body tìm thấy chính là body hiện tại.");
            }
        }

        // Release body hiện tại (quay về soul)
        if (Input.GetKeyDown(releaseKey))
        {
            Debug.Log($"[PlayerBrain] Phím [{releaseKey}] được nhấn!");
            Release();
        }
    }

    /// <summary>
    /// Possess một body mới
    /// </summary>
    public void Possess(IBody newBody)
    {
        if (newBody == null || !newBody.CanBePossessed()) return;

        possessionSystem.Possess(newBody);
        currentBody = newBody;

        if (showDebugInfo)
            Debug.Log($"[PlayerBrain] Possessed: {newBody.GetTransform().name}");
    }

    /// <summary>
    /// Release body hiện tại và quay về soul
    /// </summary>
    public void Release()
    {
        if (currentBody == null) return;

        Vector3 releasePosition = currentBody.GetTransform().position;
        possessionSystem.Release(releasePosition);
        currentBody = possessionSystem.GetSoul();

        if (showDebugInfo)
            Debug.Log("[PlayerBrain] Released to Soul form");
    }

    /// <summary>
    /// Lấy body hiện tại
    /// </summary>
    public IBody GetCurrentBody() => currentBody;

    /// <summary>
    /// Lấy transform của body hiện tại (để camera follow)
    /// </summary>
    public Transform GetCurrentTransform() => currentBody?.GetTransform();

    private void OnGUI()
    {
        if (!showDebugInfo) return;

        // Background box
        GUI.Box(new Rect(5, 5, 450, 120), "");

        // Title
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 14;
        GUI.Label(new Rect(10, 10, 400, 25), "🎮 POSSESSION SYSTEM", titleStyle);

        // Current body info
        string bodyName = currentBody?.GetTransform().name ?? "None";
        string bodyState = currentBody?.GetBodyState().ToString() ?? "None";

        GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.fontSize = 12;

        GUI.Label(new Rect(10, 35, 400, 20), $"Đang điều khiển: {bodyName}", infoStyle);
        GUI.Label(new Rect(10, 55, 400, 20), $"Trạng thái: {bodyState}", infoStyle);

        // Controls
        GUIStyle controlStyle = new GUIStyle(GUI.skin.label);
        controlStyle.fontSize = 11;
        controlStyle.normal.textColor = Color.yellow;

        GUI.Label(new Rect(10, 80, 400, 20), "🎹 WASD/Arrow = Di chuyển | Space = Nhảy | J/Click = Attack", controlStyle);
        GUI.Label(new Rect(10, 100, 400, 20), "🔄 [1] = Possess body gần nhất | [2] = Release về Soul", controlStyle);
    }
}
