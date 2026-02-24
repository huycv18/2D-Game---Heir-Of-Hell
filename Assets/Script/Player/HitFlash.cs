using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Swap Sprite (khuyên dùng)")]
    [SerializeField] private Sprite whiteSprite; // sprite màu trắng hoàn toàn

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Sprite originalSprite;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if (spriteRenderer != null)
            originalSprite = spriteRenderer.sprite;
    }

    public void TakeDamageFlash()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (whiteSprite != null)
        {
            // Tắt Animator để không override sprite trong lúc flash
            if (animator != null) animator.enabled = false;
            spriteRenderer.sprite = whiteSprite;

            yield return new WaitForSeconds(flashDuration);

            spriteRenderer.sprite = originalSprite;
            if (animator != null) animator.enabled = true;
        }
        else
        {
            // Fallback nếu chưa gán whiteSprite
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }

        flashCoroutine = null;
    }
}
