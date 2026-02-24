using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float randomOffsetX = 0.4f;

    [Header("Colors")]
    [SerializeField] private Color damageColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color healColor   = new Color(0.2f, 1f, 0.3f);

    private TextMeshProUGUI tmpText;
    private Vector3 moveDir;

    private void Awake()
    {
        // TextMeshProUGUI cho UI Canvas, fallback sang TMP_Text nếu dùng world space text
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
            Debug.LogError("[FloatingText] Không tìm thấy TextMeshProUGUI! Hãy dùng UI > Text - TextMeshPro.");
    }

    /// <summary>
    /// Khởi tạo floating text với giá trị và màu tương ứng.
    /// amount > 0 → hồi máu (xanh), amount < 0 → mất máu (đỏ)
    /// </summary>
    public void Init(int amount)
    {
        if (tmpText == null) return;

        // Text và màu
        if (amount >= 0)
        {
            tmpText.text = $"+{amount}";
            tmpText.color = healColor;
        }
        else
        {
            tmpText.text = $"{amount}";
            tmpText.color = damageColor;
        }

        // Hướng bay: lên + lệch ngang ngẫu nhiên
        float offsetX = Random.Range(-randomOffsetX, randomOffsetX);
        moveDir = new Vector3(offsetX, 1f, 0f).normalized;

        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Color startColor = tmpText.color;
        Vector3 startPos = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Bay lên theo localPosition (vì đã là child của Canvas)
            transform.localPosition = startPos + moveDir * (moveSpeed * elapsed);

            // Giữ nguyên 0.5s đầu, fade out 0.5s sau
            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            tmpText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}
