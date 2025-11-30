using UnityEngine;

public class BubbleSoundOnSpawnRandom : MonoBehaviour
{
    [Header("Configuration Audio")]
    public AudioSource audioSource;

    [Header("Sons de bulles disponibles")]
    public AudioClip[] bubbleSounds;

    // Appelé quand une bulle apparait
    public void PlayBubbleSound()
    {
        if (bubbleSounds.Length == 0 || audioSource == null)
            return;

        AudioClip randomClip = bubbleSounds[Random.Range(0, bubbleSounds.Length)];

        // Léger pitch random pour un rendu vivant
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.PlayOneShot(randomClip);
    }
}
