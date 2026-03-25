using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoSplashScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string nextSceneName = "MainMenu"; // Thay đổi tên Scene Main Menu của bạn ở đây
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float displayDuration = 2.0f;
    
    [Header("References")]
    [SerializeField] private Image logoImage;
    [SerializeField] private Image backgroundImage; // Thêm Image làm nền trắng
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        // Đảm bảo logo bắt đầu ẩn
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            // Nếu không dùng CanvasGroup, ẩn từng ảnh
            SetAlpha(0f);
        }

        StartCoroutine(SplashProcess());
    }

    private IEnumerator SplashProcess()
    {
        // 0. Đảm bảo Time.timeScale luôn bằng 1 để tránh bị đứng màn hình Logo
        Time.timeScale = 1f;

        // 1. Fade In
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);

        // 2. Chờ hiển thị
        yield return new WaitForSeconds(displayDuration);

        // 3. Fade Out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (timer / fadeDuration));
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);

        // 4. Chuyển sang Scene tiếp theo
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else 
        {
            // Cập nhật cả Logo và Nền nếu không có CanvasGroup
            if (logoImage != null)
            {
                Color c = logoImage.color;
                c.a = alpha;
                logoImage.color = c;
            }
            if (backgroundImage != null)
            {
                Color c = backgroundImage.color;
                c.a = alpha;
                backgroundImage.color = c;
            }
        }
    }
}
