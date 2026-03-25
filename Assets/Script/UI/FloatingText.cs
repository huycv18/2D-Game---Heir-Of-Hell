using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float riseTime    = 0.4f;   // thời gian bay lên
    [SerializeField] private float holdTime    = 0.2f;   // đứng yên giữa chừng
    [SerializeField] private float fadeTime    = 0.4f;   // thời gian fade out

    [Header("Movement")]
    [SerializeField] private float riseHeight  = 1.2f;   // độ cao bay lên
    [SerializeField] private float randomOffsetX = 0.3f; // lệch ngang ngẫu nhiên

    [Header("Scale Pop")]
    [SerializeField] private float punchScale  = 1.4f;   // scale to ra lúc xuất hiện
    [SerializeField] private float normalScale = 1.0f;   // scale bình thường

    [Header("Colors")]
    [SerializeField] private Color damageColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color healColor   = new Color(0.3f, 1f, 0.4f);
    [SerializeField] private Color critColor   = new Color(1f, 0.85f, 0f);  // vàng cho crit lớn

    [Header("Crit Threshold")]
    [SerializeField] private int critThreshold = 50; // damage >= threshold → dùng màu vàng + scale lớn hơn

    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void Init(int amount)
    {
        if (tmpText == null) return;

        bool isCrit = Mathf.Abs(amount) >= critThreshold;

        // Text
        if (amount >= 0)
        {
            tmpText.text  = $"+{amount}";
            tmpText.color = healColor;
        }
        else
        {
            tmpText.text  = isCrit ? $"{amount}!" : $"{amount}";
            tmpText.color = isCrit ? critColor : damageColor;
        }

        // Lệch ngang ngẫu nhiên nhẹ
        float offsetX = Random.Range(-randomOffsetX, randomOffsetX);
        transform.localPosition += new Vector3(offsetX, 0f, 0f);

        // Scale punch lớn hơn cho crit
        float startScale = isCrit ? punchScale * 1.2f : punchScale;
        transform.localScale = Vector3.one * startScale;

        StartCoroutine(AnimateRoutine(startScale));
    }

    private IEnumerator AnimateRoutine(float startScale)
    {
        Color baseColor    = tmpText.color;
        Vector3 startPos   = transform.localPosition;
        Vector3 targetPos  = startPos + new Vector3(0f, riseHeight, 0f);

        // ── Phase 1: Bay lên với Ease Out + Scale thu về normal ──
        float elapsed = 0f;
        while (elapsed < riseTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseTime;

            // Ease Out Cubic: nhanh lúc đầu, chậm dần
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            transform.localPosition = Vector3.LerpUnclamped(startPos, targetPos, easedT);

            // Scale: punch → normal với overshoot nhỏ (elastic feel)
            float scaleT   = Mathf.Clamp01(t / 0.5f); // hoàn thành trong nửa đầu
            float scaleVal = Mathf.LerpUnclamped(startScale, normalScale,
                                EaseOutBack(scaleT));
            transform.localScale = Vector3.one * scaleVal;

            tmpText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localScale    = Vector3.one * normalScale;

        // ── Phase 2: Đứng yên ──
        yield return new WaitForSeconds(holdTime);

        // ── Phase 3: Fade out + trôi lên nhẹ thêm ──
        elapsed = 0f;
        Vector3 fadeStartPos = transform.localPosition;
        Vector3 fadeEndPos   = fadeStartPos + new Vector3(0f, riseHeight * 0.3f, 0f);

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;

            // Ease In: chậm đầu, nhanh cuối
            float easedT = t * t;
            transform.localPosition = Vector3.LerpUnclamped(fadeStartPos, fadeEndPos, easedT);

            float alpha = 1f - t;
            tmpText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    // Ease Out Back — scale có overshoot nhỏ cho cảm giác bouncy
    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
