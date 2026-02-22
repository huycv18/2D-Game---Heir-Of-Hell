using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentEnergy;

    [SerializeField] private int energyThreshold = 3;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject enemySpaner;

    private bool bossCalled = false;

    void Start()
    {
        boss.SetActive(false);
    }

    void Update()
    {
        
    }

    public void AddEnergy()
    {
        if (bossCalled)
        {
            return;
        }

        currentEnergy += 1;

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
    }
}