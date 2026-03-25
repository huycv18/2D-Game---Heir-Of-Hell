using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int currentEnergy;
    private int score;

    [SerializeField] private int energyThreshold = 3;
    [SerializeField] private GameObject boss;
    [SerializeField] private RoomSpawner roomSpawner;
    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject energyBarUI;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gameWinMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private AudioManager audioManager;

    private bool bossCalled = false;
    public static bool playAgain = false;

    void Start()
    {
        currentEnergy = 0;
        score = 0;

        boss.SetActive(false);
        UpdateEnergyBar();
        UpdateScoreUI();

        StartGame();
    }

    // ================= SCORE =================

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    // ================= ENERGY =================

    public void AddEnergy()
    {
        if (bossCalled) return;

        currentEnergy += 1;
        UpdateEnergyBar();

        if (currentEnergy == energyThreshold)
        {
            CallBoss();
        }
    }

    private void CallBoss()
    {
        bossCalled = true;
        boss.SetActive(true);
        roomSpawner?.StopSpawner();
        if (energyBarUI != null) energyBarUI.SetActive(false);
        audioManager.PlayBossAudio();
    }

    private void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentEnergy / energyThreshold);
            energyBar.fillAmount = fillAmount;
        }
    }

    // ================= MENU =================

    public void GameOverMenu()
    {
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
        gameWinMenu.SetActive(false);
    }

    public void PauseGameMenu()
    {
        pauseMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        Time.timeScale = 0f;
        gameWinMenu.SetActive(false);
    }

    public void WinGame()
    {
        if (gameWinMenu != null) gameWinMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        if (gameWinMenu != null) gameWinMenu.SetActive(false);
        gameUI.SetActive(true);
        if (energyBarUI != null) energyBarUI.SetActive(true);
        Time.timeScale = 1f;
        audioManager.PlayDefaultAudio();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        if (gameWinMenu != null) gameWinMenu.SetActive(false);
        gameUI.SetActive(true);
        if (energyBarUI != null) energyBarUI.SetActive(true);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}