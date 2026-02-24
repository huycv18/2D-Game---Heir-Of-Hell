using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mỗi SpawnPoint có 1 loại Enemy riêng.
/// Load scene → spawn tất cả ngay lập tức.
/// Dọn sạch hết → chờ delayBetweenWaves → spawn lại toàn bộ.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSlot
    {
        public Transform spawnPoint;
        public GameObject enemyPrefab;
    }

    [Header("Spawn Slots - mỗi slot = 1 vị trí + 1 loại enemy")]
    [SerializeField] private SpawnSlot[] slots;

    [Header("Wave Settings")]
    [SerializeField] private float delayBetweenWaves = 3f;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();
    private bool isWaiting = false;

    private void Start()
    {
        SpawnAll();
    }

    private void SpawnAll()
    {
        isWaiting = false;
        aliveEnemies.Clear();

        foreach (SpawnSlot slot in slots)
        {
            if (slot.spawnPoint == null || slot.enemyPrefab == null) continue;

            GameObject obj = Instantiate(slot.enemyPrefab, slot.spawnPoint.position, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                aliveEnemies.Add(enemy);
                enemy.OnDeath += OnEnemyDied;
            }
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count == 0 && !isWaiting && gameObject.activeInHierarchy)
        {
            isWaiting = true;
            StartCoroutine(DelayNextWave());
        }
    }

    private IEnumerator DelayNextWave()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        SpawnAll();
    }

    public void StopSpawner()
    {
        StopAllCoroutines();
        isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        if (slots == null) return;
        Gizmos.color = Color.cyan;
        foreach (SpawnSlot slot in slots)
            if (slot?.spawnPoint != null)
                Gizmos.DrawWireSphere(slot.spawnPoint.position, 0.4f);
    }
}