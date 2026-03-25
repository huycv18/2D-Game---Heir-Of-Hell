using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Tooltip("Tên của Scene sẽ dùng làm màn hình Loading")]
    [SerializeField] private string loadingSceneName = "LoadingScene";
    
    [Tooltip("Thời gian chờ tối thiểu (giây) trước khi tắt loading screen")]
    [SerializeField] private float minLoadingTime = 1.5f;

    private string targetSceneName;
    private int targetSceneIndex;
    private bool isLoadingByIndex;
    
    // Thêm biến kiểm tra đang load trạng thái
    private bool isCurrentlyLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (isCurrentlyLoading) return; // Chặn bấm đúp nhiều lần
        isCurrentlyLoading = true;
        
        targetSceneName = sceneName;
        isLoadingByIndex = false;
        StartCoroutine(LoadSceneProcess());
    }

    public void LoadScene(int sceneIndex)
    {
        if (isCurrentlyLoading) return; // Chặn bấm đúp
        isCurrentlyLoading = true;

        targetSceneIndex = sceneIndex;
        isLoadingByIndex = true;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        // 0. Đảm bảo thời gian chạy bình thường để không bị đứng animation
        Time.timeScale = 1f;

        // Phát nhạc Loading
        AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
        audioManager?.PlayLoadingMusic();

        // 1. Chuyển ngay sang Loading Scene (Synchronous)
        SceneManager.LoadScene(loadingSceneName);
        
        // 2. Đợi 2 frame để cấu trúc render của Scene cũ hoàn toàn bị xóa sạch
        yield return null;
        yield return null;

        float elapsedTime = 0f;

        // 3. Bắt đầu load ngầm Scene tiếp theo
        AsyncOperation operation = isLoadingByIndex ? 
            SceneManager.LoadSceneAsync(targetSceneIndex) : 
            SceneManager.LoadSceneAsync(targetSceneName);
            
        // Chặn không cho Scene mới tự động nhảy vào thay thế Loading Scene ngay lập tức
        operation.allowSceneActivation = false;

        // 4. Chờ cho đến khi load xong (progress >= 0.9) và đạt thời gian tối thiểu
        while (operation.progress < 0.9f || elapsedTime < minLoadingTime)
        {
            // Dùng unscaledDeltaTime để tránh bị đứng nếu Time.timeScale = 0 (khi Pause/GameOver)
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 5. Cho phép Scene mới kích hoạt
        operation.allowSceneActivation = true;

        // Đợi đến khi Scene mới thực sự được load xong hoàn toàn
        while (!operation.isDone)
        {
            yield return null;
        }

        // 6. Giải phóng trạng thái để có thể load lần sau
        isCurrentlyLoading = false;
    }
}
