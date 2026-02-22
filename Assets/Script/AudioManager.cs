using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudioSource;
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
}