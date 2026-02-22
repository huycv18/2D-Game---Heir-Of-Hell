using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioSource defaultAudioSource;
    [SerializeField] private AudioSource bossAudioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reLoadClip;
    [SerializeField] private AudioClip energyClip;

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