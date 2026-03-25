using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn vào GameObject GameWin trong Canvas.
/// Xử lý các nút: Play Again, Main Menu, Quit.
/// </summary>
public class GameWin : MonoBehaviour
{
    [Tooltip("Tên scene đầu tiên để Play Again")]
    [SerializeField] private string firstScene = "1";
    [Tooltip("Tên scene Main Menu")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    /// <summary>Nút Play Again → load lại từ scene 1.</summary>
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(firstScene);
        else SceneManager.LoadScene(firstScene);
    }

    /// <summary>Nút Main Menu → về màn hình chính.</summary>
    public void MainMenu()
    {
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(mainMenuScene);
        else SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>Nút Quit → thoát game.</summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void ReplayFromLevel1()
    {
        GameManager.playAgain = true;
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(firstScene);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(firstScene);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(mainMenuScene);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
    }
}
