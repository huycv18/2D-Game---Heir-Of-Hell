using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void ContinueGame()
    {
        gameManager.ResumeGame();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene("MainMenu");
        else SceneManager.LoadScene("MainMenu");
    }

    public void ReplayGame()
    {
        GameManager.playAgain = true;
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetryGame()
    {
        GameManager.playAgain = true;
        Time.timeScale = 1f;
        if (LoadingManager.Instance != null) LoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
