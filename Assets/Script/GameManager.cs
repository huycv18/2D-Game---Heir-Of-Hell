using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private int currentEnergy = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int energyThreshold = 3;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject gameUI;

    private bool bossCalled = false;

    private void Start()
    {
        UpdateScore();
        UpdateEnergyBar();

        if (boss != null)
            boss.SetActive(false);
    }

    // ===================== SCORE =====================

    public void AddScore(int points)
    {
        score += points;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    // ===================== ENERGY =====================

    public void AddEnergy()
    {
        if (bossCalled)
            return;

        currentEnergy++;
        UpdateEnergyBar();

        if (currentEnergy >= energyThreshold)
        {
            CallBoss();
        }
    }

    private void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentEnergy / energyThreshold);
            energyBar.fillAmount = fillAmount;
        }
    }

    // ===================== BOSS =====================

    private void CallBoss()
    {
        bossCalled = true;

        if (boss != null)
            boss.SetActive(true);

        if (enemySpawner != null)
            enemySpawner.SetActive(false);

        if (gameUI != null)
            gameUI.SetActive(false);
    }
}