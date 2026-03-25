using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    [Header("Waves - kéo Enemy từ Hierarchy vào đây")]
    [SerializeField] private WaveData[] waves;

    [Header("Settings")]
    [SerializeField] private float delayBetweenWaves = 3f;

    [Header("Trigger Zone - vùng kích hoạt khi Player bước vào")]
    [SerializeField] private bool activateOnPlayerEnter = true;
    [Tooltip("Kích thước vùng trigger (chỉnh trong Scene View)")]
    [SerializeField] private Vector2 triggerSize = new Vector2(10f, 5f);
    [SerializeField] private Vector2 triggerOffset = Vector2.zero;

    [Header("Music Settings")]
    [Tooltip("Nếu tích, nhạc Action sẽ phát ngay khi chạm Trigger. Nếu bỏ, nhạc chỉ phát khi Enemy ĐẦU TIÊN thực sự xuất hiện.")]
    [SerializeField] private bool playMusicOnTrigger = false;

    public System.Action OnAllWavesCleared;

    private int currentWaveIndex = 0;
    private readonly List<Enemy> aliveEnemies = new List<Enemy>();
    private bool isWaiting = false;
    private bool isActivated = false;

    private void Start()
    {
        // Nếu không dùng trigger → spawn ngay khi load
        if (!activateOnPlayerEnter)
        {
            // Phát nhạc Action ngay lập tức nếu quái hiện ra từ đầu game
            AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
            audioManager?.PlayActionMusic();
            
            StartWave(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activateOnPlayerEnter) return;
        if (isActivated) return;
        if (!collision.CompareTag("Player")) return;

        isActivated = true;
        
        // Phát nhạc ngay lập tức nếu được cấu hình
        if (playMusicOnTrigger)
        {
            AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
            audioManager?.PlayActionMusic();
        }

        StartWave(0);
    }

    private void StartWave(int index)
    {
        if (waves == null || index >= waves.Length) return;

        currentWaveIndex = index;
        isWaiting = false;
        aliveEnemies.Clear();

        WaveData wave = waves[index];
        if (wave?.enemies == null) return;

        // BỎ đoạn phát nhạc tại đây để tránh phát ngay khi vừa spawn
        /*  
        if (!playMusicOnTrigger)
        {
            AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
            audioManager?.PlayActionMusic();
        }
        */

        foreach (GameObject enemyObj in wave.enemies)
        {
            if (enemyObj == null) continue;

            enemyObj.SetActive(true);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                aliveEnemies.Add(enemy);
                enemy.OnDeath += OnEnemyDied;
            }
        }

        Debug.Log($"[{gameObject.name}] Wave {index + 1}/{waves.Length} — {aliveEnemies.Count} enemy");
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDied;
        aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count > 0 || isWaiting) return;

        int next = currentWaveIndex + 1;
        if (next < waves.Length)
        {
            isWaiting = true;
            StartCoroutine(DelayNextWave(next));
        }
        else
        {
            Debug.Log($"[{gameObject.name}] ✓ Tất cả wave đã clear!");
            
            // Quay về nhạc mặc định khi dọn sạch quái trong phòng
            AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
            audioManager?.PlayDefaultAudio();

            OnAllWavesCleared?.Invoke();
        }
    }

    private IEnumerator DelayNextWave(int nextIndex)
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        StartWave(nextIndex);
    }

    public void StopSpawner()
    {
        StopAllCoroutines();
        isWaiting = false;
        foreach (Enemy e in aliveEnemies)
            if (e != null) e.OnDeath -= OnEnemyDied;
        aliveEnemies.Clear();
    }

    // ── Gizmos ───────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        // Vẽ vùng trigger
        if (activateOnPlayerEnter)
        {
            Gizmos.color = isActivated ? new Color(0, 1, 0, 0.2f) : new Color(0, 0.5f, 1f, 0.2f);
            Gizmos.DrawCube(
                transform.position + (Vector3)triggerOffset,
                new Vector3(triggerSize.x, triggerSize.y, 0.1f)
            );
            Gizmos.color = isActivated ? Color.green : Color.cyan;
            Gizmos.DrawWireCube(
                transform.position + (Vector3)triggerOffset,
                new Vector3(triggerSize.x, triggerSize.y, 0.1f)
            );
        }

        // Vẽ vị trí enemy theo màu wave
        if (waves == null) return;
        Color[] colors = { Color.cyan, Color.green, Color.yellow, Color.magenta, Color.red };
        for (int w = 0; w < waves.Length; w++)
        {
            if (waves[w]?.enemies == null) continue;
            Gizmos.color = colors[w % colors.Length];
            foreach (GameObject e in waves[w].enemies)
            {
                if (e == null) continue;
                Gizmos.DrawWireSphere(e.transform.position, 0.5f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    e.transform.position + Vector3.up * 0.7f,
                    $"[{gameObject.name}] W{w + 1}: {e.name}"
                );
#endif
            }
        }
    }

    // Đồng bộ BoxCollider2D với triggerSize khi thay đổi trong Inspector
    private void OnValidate()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;
        col.isTrigger = true;
        col.offset = triggerOffset;
        col.size = triggerSize;
    }
}
