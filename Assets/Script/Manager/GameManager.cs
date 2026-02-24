using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int currentEnergy;
    private int score;

    [SerializeField] private int energyThreshold = 3;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject enemySpaner;
    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject energyBarUI;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
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

        if (playAgain)
        {
            playAgain = false;
            StartGame();
        }
        else
        {
            MainMenu();
            audioManager.StopAudioGame();
        }
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
        enemySpaner.SetActive(false);
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

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void GameOverMenu()
    {
        gameOverMenu.SetActive(true);
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PauseGameMenu()
    {
        pauseMenu.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameUI.SetActive(true);
        if (energyBarUI != null) energyBarUI.SetActive(true);
        Time.timeScale = 1f;
        audioManager.PlayDefaultAudio();
    }

    public void ResumeGame()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameUI.SetActive(true);
        if (energyBarUI != null) energyBarUI.SetActive(true);
        Time.timeScale = 1f;
    }
}