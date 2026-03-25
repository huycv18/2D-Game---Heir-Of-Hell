using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Hiển thị Tutorial dạng slideshow sprites trên UI (Foreground overlay).
/// - Tự động hiện lúc bắt đầu hoặc bật/tắt bằng phím T.
/// - Chuyển slide bằng chuột trái / phím mũi tên / A-D.
/// - Nhấn ESC hoặc hết slide → tự đóng.
/// </summary>
public class TutorialController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel chứa toàn bộ Tutorial (overlay)")]
    [SerializeField] private GameObject tutorialPanel;
    [Tooltip("Image dùng để hiển thị sprite slide")]
    [SerializeField] private Image slideImage;
    [Tooltip("Nút Next (tùy chọn)")]
    [SerializeField] private GameObject nextButton;
    [Tooltip("Nút Prev (tùy chọn)")]
    [SerializeField] private GameObject prevButton;
    [Tooltip("Nút Close / Skip")]
    [SerializeField] private GameObject closeButton;

    [Header("Slides")]
    [Tooltip("Kéo các sprite Tutorial vào đây theo thứ tự")]
    [SerializeField] private Sprite[] slides;

    [Header("Animation")]
    [Tooltip("Thời gian fade giữa các slide (giây)")]
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Settings")]
    [Tooltip("Tự động hiện Tutorial khi bắt đầu scene")]
    [SerializeField] private bool showOnStart = true;
    [Tooltip("Dừng thời gian khi Tutorial mở")]
    [SerializeField] private bool pauseGameWhileOpen = false;
    [Tooltip("Phím bật/tắt Tutorial")]
    [SerializeField] private KeyCode toggleKey = KeyCode.T;

    private int currentIndex = 0;
    private bool isOpen = false;
    private Coroutine fadeCoroutine;

    // ──────────────────────────────────────────────

    private void Start()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        if (showOnStart && slides != null && slides.Length > 0)
            Open();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen) Close();
            else        Open();
        }

        if (!isOpen) return;

        // Chuyển slide bằng bàn phím
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextSlide();
        if (Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.A))
            PrevSlide();
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    // ──────────────────────────────────── PUBLIC ────

    /// <summary>Mở Tutorial từ slide đầu tiên.</summary>
    public void Open()
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("[Tutorial] Chưa có slide nào! Hãy kéo sprites vào mảng Slides.");
            return;
        }

        currentIndex = 0;
        isOpen = true;
        tutorialPanel?.SetActive(true);

        if (pauseGameWhileOpen) Time.timeScale = 0f;

        ShowSlide(currentIndex, false);
        UpdateButtons();
    }

    /// <summary>Đóng Tutorial.</summary>
    public void Close()
    {
        isOpen = false;
        tutorialPanel?.SetActive(false);

        if (pauseGameWhileOpen) Time.timeScale = 1f;
    }

    /// <summary>Gọi từ nút Next.</summary>
    public void NextSlide()
    {
        if (currentIndex >= slides.Length - 1)
        {
            Close(); // hết slide → tự đóng
            return;
        }

        currentIndex++;
        ShowSlide(currentIndex, true);
        UpdateButtons();
    }

    /// <summary>Gọi từ nút Prev.</summary>
    public void PrevSlide()
    {
        if (currentIndex <= 0) return;

        currentIndex--;
        ShowSlide(currentIndex, true);
        UpdateButtons();
    }

    // ──────────────────────────────────── PRIVATE ───

    private void ShowSlide(int index, bool animate)
    {
        if (slideImage == null || slides == null || index >= slides.Length) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (animate)
            fadeCoroutine = StartCoroutine(FadeToSlide(slides[index]));
        else
        {
            slideImage.sprite = slides[index];
            SetAlpha(1f);
        }
    }

    private IEnumerator FadeToSlide(Sprite nextSprite)
    {
        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - t / fadeDuration);
            yield return null;
        }

        slideImage.sprite = nextSprite;

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(t / fadeDuration);
            yield return null;
        }

        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        if (slideImage == null) return;
        Color c = slideImage.color;
        c.a = Mathf.Clamp01(alpha);
        slideImage.color = c;
    }

    private void UpdateButtons()
    {
        if (prevButton  != null) prevButton.SetActive(currentIndex > 0);
        if (nextButton  != null) nextButton.SetActive(true); // luôn hiện, slide cuối sẽ Close
        if (closeButton != null) closeButton.SetActive(true);
    }
}

