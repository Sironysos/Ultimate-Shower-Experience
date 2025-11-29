using UnityEngine;

public class BubbleSoundOnSpawnRandom : MonoBehaviour
{
    public ParticleSystem ps;
    public AudioSource audioSource;

    [Header("Liste des sons de bulles")]
    public AudioClip[] bubbleSounds;

    private int lastParticleCount = 0;

    void Start()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        int currentCount = ps.particleCount;

        if (currentCount > lastParticleCount)
        {
            // Une ou plusieurs particules viennent de spawn
            if (bubbleSounds.Length > 0)
            {
                AudioClip randomClip = bubbleSounds[Random.Range(0, bubbleSounds.Length)];

                // On varie un peu le pitch pour un rendu plus naturel
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(randomClip);
            }
        }

        lastParticleCount = currentCount;
    }
}
