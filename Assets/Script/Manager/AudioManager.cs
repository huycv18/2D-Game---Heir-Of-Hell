using UnityEngine;

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

    [Header("Impact SFX (random 1 in 3)")]
    [SerializeField] private AudioClip[] impactClips;

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

    public void PlayImpactSound()
    {
        if (impactClips == null || impactClips.Length == 0) return;
        AudioClip clip = impactClips[Random.Range(0, impactClips.Length)];
        if (clip != null)
            effectAudioSource.PlayOneShot(clip);
    }

    public void PlayDefaultAudio()
    {
        bossAudioSource.Stop();
        defaultAudioSource.Play();
    }

    public void PlayBossAudio()
    {
        defaultAudioSource.Stop();
        bossAudioSource.Play();
    }

    public void StopAudioGame()
    {
        effectAudioSource.Stop();
        bossAudioSource.Stop();
        defaultAudioSource.Stop();
    }
}

