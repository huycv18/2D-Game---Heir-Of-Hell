using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Tooltip("Tên scene level 1 hoặc tutorial để bắt đầu")]
    [SerializeField] private string firstScene = "0Tutorial";

    private void Start()
    {
        AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
        audioManager?.PlayMenuMusic();
    }

    public void StartGame()
    {
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(firstScene);
        }
        else
        {
            SceneManager.LoadScene(firstScene);
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBGL
            Application.OpenURL("about:blank"); // Hoặc điều hướng về trang chủ của bạn
        #else
            Application.Quit();
        #endif
        Debug.Log("Quit Game!");
    }
}
