using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Quản lý logic possess/release và tìm kiếm body
/// </summary>
public class PossessionSystem : MonoBehaviour
{
    [Header("Soul Settings")]
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private float possessionRange = 3f;
    [SerializeField] private float possessionCooldown = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private bool showPossessionRange = true;
    [SerializeField] private Color rangeColor = new Color(0, 1, 0, 0.3f);

    private IBody currentBody;
    private SoulBody soul;
    private float lastPossessionTime = -999f; // Khởi tạo âm để bypass cooldown lần đầu

    private void Awake()
    {
        // Tạo soul nếu chưa có
        if (soul == null && soulPrefab != null)
        {
            GameObject soulObj = Instantiate(soulPrefab, transform.position, Quaternion.identity);
            soul = soulObj.GetComponent<SoulBody>();
            if (soul != null)
                soulObj.SetActive(false); // Ẩn soul ban đầu
        }
    }

    /// <summary>
    /// Possess một body mới
    /// </summary>
    public void Possess(IBody newBody)
    {
        if (newBody == null)
        {
            Debug.LogError("[PossessionSystem] newBody is NULL!");
            return;
        }

        if (!CanPossessNow())
        {
            Debug.LogWarning($"[PossessionSystem] Cooldown chưa hết! Còn {possessionCooldown - (Time.time - lastPossessionTime):F2}s");
            return;
        }

        Debug.Log($"[PossessionSystem] Possessing: {newBody.GetTransform().name}");

        // Release body cũ nếu có
        if (currentBody != null && !ReferenceEquals(currentBody, soul))
        {
            Debug.Log($"[PossessionSystem] Releasing old body: {currentBody.GetTransform().name}");
            currentBody.OnReleased();
        }

        // Ẩn soul
        if (soul != null)
        {
            Debug.Log("[PossessionSystem] Hiding soul");
            soul.gameObject.SetActive(false);
        }

        // Possess body mới
        newBody.OnPossessed();
        currentBody = newBody;
        lastPossessionTime = Time.time;

        Debug.Log($"[PossessionSystem] Possession complete! Current body: {currentBody.GetTransform().name}");
    }

    /// <summary>
    /// Release body hiện tại và kích hoạt soul
    /// </summary>
    public void Release(Vector3 position)
    {
        if (currentBody != null && !ReferenceEquals(currentBody, soul))
        {
            currentBody.OnReleased();
        }

        // Kích hoạt soul tại vị trí release
        if (soul != null)
        {
            soul.transform.position = position;
            soul.gameObject.SetActive(true);
            soul.OnPossessed();
            currentBody = soul;
        }

        lastPossessionTime = Time.time;
    }

    /// <summary>
    /// Tìm body gần nhất có thể possess
    /// </summary>
    public IBody FindNearestPossessableBody(Vector3 fromPosition)
    {
        IBody[] allBodies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IBody>()
            .ToArray();

        Debug.Log($"[PossessionSystem] Tìm thấy {allBodies.Length} IBody trong scene");

        IBody nearest = null;
        float minDistance = possessionRange;

        foreach (IBody body in allBodies)
        {
            string bodyName = body.GetTransform().name;
            bool canPossess = body.CanBePossessed();
            bool isCurrent = body == currentBody;
            float distance = Vector3.Distance(fromPosition, body.GetTransform().position);

            Debug.Log($"[PossessionSystem] Body: {bodyName} | Distance: {distance:F2} | CanPossess: {canPossess} | IsCurrent: {isCurrent}");

            if (!canPossess) continue;
            if (isCurrent) continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = body;
            }
        }

        if (nearest != null)
        {
            Debug.Log($"[PossessionSystem] Body gần nhất: {nearest.GetTransform().name} (distance: {minDistance:F2})");
        }
        else
        {
            Debug.LogWarning($"[PossessionSystem] Không tìm thấy body nào trong tầm {possessionRange}m");
        }

        return nearest;
    }

    /// <summary>
    /// Lấy danh sách tất cả body trong tầm
    /// </summary>
    public List<IBody> GetBodiesInRange(Vector3 fromPosition)
    {
        List<IBody> bodiesInRange = new List<IBody>();

        IBody[] allBodies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IBody>()
            .ToArray();

        foreach (IBody body in allBodies)
        {
            if (!body.CanBePossessed()) continue;
            if (body == currentBody) continue;

            float distance = Vector3.Distance(fromPosition, body.GetTransform().position);
            if (distance <= possessionRange)
            {
                bodiesInRange.Add(body);
            }
        }

        return bodiesInRange;
    }

    /// <summary>
    /// Kiểm tra có thể possess ngay bây giờ không (cooldown)
    /// </summary>
    private bool CanPossessNow()
    {
        return Time.time - lastPossessionTime >= possessionCooldown;
    }

    /// <summary>
    /// Lấy soul body
    /// </summary>
    public IBody GetSoul() => soul;

    /// <summary>
    /// Lấy body hiện tại
    /// </summary>
    public IBody GetCurrentBody() => currentBody;

    private void OnDrawGizmos()
    {
        if (!showPossessionRange || currentBody == null) return;

        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(currentBody.GetTransform().position, possessionRange);
    }
}
