using UnityEngine;

/// <summary>
/// Singleton quản lý spawn FloatingText.
/// Gọi FloatingTextManager.Instance.ShowValue(amount, position) từ bất kỳ đâu.
/// </summary>
public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas worldCanvas; // Canvas chế độ World Space

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Spawn floating text tại vị trí world.
    /// amount > 0 → xanh (+heal), amount < 0 → đỏ (-damage)
    /// </summary>
    public void ShowValue(int amount, Vector3 worldPosition)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("[FloatingTextManager] floatingTextPrefab chưa được gán!");
            return;
        }

        // Spawn là child của Canvas ngay từ đầu
        Transform parent = worldCanvas != null ? worldCanvas.transform : transform;
        GameObject obj = Instantiate(floatingTextPrefab, parent);

        // Chuyển world position → local position trong Canvas
        obj.transform.position = worldPosition + new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(0.2f, 0.5f),
            0f
        );

        FloatingText ft = obj.GetComponent<FloatingText>();
        ft?.Init(amount);
    }
}
