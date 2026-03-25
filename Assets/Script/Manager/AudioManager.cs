using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioSource defaultAudioSource;
    [SerializeField] private AudioSource bossAudioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reLoadClip;
    [SerializeField] private AudioClip energyClip;
    [SerializeField] private AudioClip coinClip;

    [Header("New Sound Effects")]
    [SerializeField] private AudioClip enemyExplosionClip;
    [SerializeField] private AudioClip gateOpenClip;
    [SerializeField] private AudioClip playerJumpClip;
    [SerializeField] private AudioClip playerCombatClip;
    [SerializeField] private AudioClip bowShootClip;
    [SerializeField] private AudioClip bowReloadClip;

    [Header("Impact SFX (random 1 in 3)")]
    [SerializeField] private AudioClip[] impactClips;

    [Header("UI & Menu Sounds")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip loadingMusic;
    [SerializeField] private AudioClip actionMusic;     // Nhạc chiến đấu khi gặp quái
    [SerializeField] private AudioClip winSFX;        // SFX Thông báo Win (phát 1 lần khi Boss chết)
    [SerializeField] private AudioClip afterWinMusic; // Nhạc du dương phát sau khi thắng (Loop)
    [SerializeField] private AudioSource menuAudioSource;

    public void PlayShootSound()
    {
        effectAudioSource.PlayOneShot(shootClip);
    }

    public void PlayReLoadSound()
    {
        effectAudioSource.PlayOneShot(reLoadClip);
    }

    public void PlayEnergySound()
    {
        effectAudioSource.PlayOneShot(energyClip);
    }

    public void PlayCoinSound()
    {
        effectAudioSource.PlayOneShot(coinClip);
    }

    public void PlayEnemyExplosionSound()
    {
        if (enemyExplosionClip != null)
            effectAudioSource.PlayOneShot(enemyExplosionClip);
    }

    public void PlayGateOpenSound()
    {
        if (gateOpenClip != null)
            effectAudioSource.PlayOneShot(gateOpenClip);
    }

    public void PlayPlayerJumpSound()
    {
        if (playerJumpClip != null)
            effectAudioSource.PlayOneShot(playerJumpClip);
    }

    public void PlayPlayerCombatSound()
    {
        if (playerCombatClip != null)
            effectAudioSource.PlayOneShot(playerCombatClip);
    }

    public void PlayBowShootSound()
    {
        if (bowShootClip != null)
            effectAudioSource.PlayOneShot(bowShootClip);
    }

    public void PlayBowReloadSound()
    {
        if (bowReloadClip != null)
            effectAudioSource.PlayOneShot(bowReloadClip);
    }

    public void PlayImpactSound()
    {
        if (impactClips == null || impactClips.Length == 0) return;
        AudioClip clip = impactClips[Random.Range(0, impactClips.Length)];
        if (clip != null)
            effectAudioSource.PlayOneShot(clip);
    }

    public void PlayMenuMusic()
    {
        if (menuAudioSource == null || menuMusic == null) return;
        StopAllMusic();
        menuAudioSource.clip = menuMusic;
        menuAudioSource.loop = true;
        menuAudioSource.Play();
    }

    public void PlayLoadingMusic()
    {
        if (menuAudioSource == null || loadingMusic == null) return;
        StopAllMusic();
        menuAudioSource.clip = loadingMusic;
        menuAudioSource.loop = true;
        menuAudioSource.Play();
    }

    public void PlayWinMusic()
    {
        if (menuAudioSource == null) return;
        StopAllMusic();
        
        // 1. Phát SFX Thông báo Win (tiếng hiệu ứng thắng trận)
        if (winSFX != null)
            effectAudioSource.PlayOneShot(winSFX);

        // 2. Phát Nhạc After Win (nhạc nền du dương sau khi thắng)
        if (afterWinMusic != null)
        {
            menuAudioSource.clip = afterWinMusic;
            menuAudioSource.loop = true;
            menuAudioSource.Play();
        }
    }

    public void PlayActionMusic()
    {
        if (defaultAudioSource == null || actionMusic == null) return;
        
        // NẾU ĐANG PHÁT NHẠC BOSS HOẶC NHẠC CHIẾN THẮNG THÌ KHÔNG ĐÈ LÊN
        if ((bossAudioSource != null && bossAudioSource.isPlaying) || 
            (menuAudioSource != null && (menuAudioSource.clip == afterWinMusic && menuAudioSource.isPlaying))) 
            return;

        // Nếu bản nhạc chiến đấu đang chạy rồi thì không khởi động lại để tránh bị vấp nhạc
        if (defaultAudioSource.clip == actionMusic && defaultAudioSource.isPlaying)
        {
            // Nếu có lệnh đếm ngược tắt nhạc đang chạy, hãy hủy nó vì quái lại xuất hiện
            if (actionMusicTimeoutCoroutine != null)
            {
                StopCoroutine(actionMusicTimeoutCoroutine);
                actionMusicTimeoutCoroutine = null;
            }
            return;
        }

        StopAllMusic();
        defaultAudioSource.clip = actionMusic;
        defaultAudioSource.loop = true;
        defaultAudioSource.Play();
    }

    public void PlayBossAudio()
    {
        // Nhạc Boss là nhạc ưu tiên cao nhất
        StopAllMusic();
        if (bossAudioSource != null)
        {
            bossAudioSource.loop = true;
            bossAudioSource.Play();
        }
    }

    private void StopAllMusic()
    {
        if (defaultAudioSource != null) defaultAudioSource.Stop();
        if (bossAudioSource != null) bossAudioSource.Stop();
        if (menuAudioSource != null) menuAudioSource.Stop();
    }

    public void PlayDefaultAudio()
    {
        StopAllMusic();
        defaultAudioSource.Play();
    }

    public void StopAudioGame()
    {
        effectAudioSource.Stop();
        bossAudioSource.Stop();
        defaultAudioSource.Stop();
    }

    private Coroutine actionMusicTimeoutCoroutine;

    public void StartActionMusicTimeout(float delay)
    {
        // Nhạc Boss đang phát thì không cần đếm ngược tắt nhạc Action
        if (bossAudioSource != null && bossAudioSource.isPlaying) return;

        if (actionMusicTimeoutCoroutine != null)
            StopCoroutine(actionMusicTimeoutCoroutine);
        
        actionMusicTimeoutCoroutine = StartCoroutine(ActionMusicTimeoutProcess(delay));
    }

    private IEnumerator ActionMusicTimeoutProcess(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            // Nếu nhạc Boss hoặc nhạc Win bắt đầu phát, tắt nhạc Action ngay lập tức và thoát
            if ((bossAudioSource != null && bossAudioSource.isPlaying) || 
                (menuAudioSource != null && menuAudioSource.clip == afterWinMusic && menuAudioSource.isPlaying))
            {
                if (defaultAudioSource.clip == actionMusic) defaultAudioSource.Stop();
                actionMusicTimeoutCoroutine = null;
                yield break;
            }

            // Kiểm tra xem có bất kỳ Enemy nào đang Aggro trong Camera không
            if (IsAnyEnemyAggro())
            {
                actionMusicTimeoutCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Hết thời gian mà không có ai aggro -> Quay về nhạc mặc định
        if (defaultAudioSource.clip == actionMusic)
        {
            PlayDefaultAudio();
        }
        actionMusicTimeoutCoroutine = null;
    }

    private bool IsAnyEnemyAggro()
    {
        Enemy[] allEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Camera mainCam = Camera.main;
        if (mainCam == null) return false;

        // Tính toán các mặt phẳng Viewport để kiểm tra xem Object có nằm trong Camera không
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);

        foreach (Enemy e in allEnemies)
        {
            if (e != null && e.gameObject.activeInHierarchy)
            {
                // Kiểm tra xem Enemy có đang ở trạng thái hung hãn (Aggro) không
                if (e.GetCurrentState() == Enemy.EnemyState.Chase || 
                    e.GetCurrentState() == Enemy.EnemyState.Attack || 
                    e.GetCurrentState() == Enemy.EnemyState.Alert)
                {
                    // KIỂM TRA TRONG TẦM CAMERA: Nếu Enemy đang aggro VÀ nằm trong khung hình
                    Collider2D enemyCollider = e.GetComponent<Collider2D>();
                    if (enemyCollider != null)
                    {
                        if (GeometryUtility.TestPlanesAABB(planes, enemyCollider.bounds))
                        {
                            return true; // Có quái đang đuổi theo trong tầm mắt
                        }
                    }
                }
            }
        }
        return false;
    }
}
