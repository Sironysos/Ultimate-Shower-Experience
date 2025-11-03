using UnityEngine;
using System.Collections;

public class ShowerToggle : MonoBehaviour
{
    public ParticleSystem showerParticles;
    public float showerDuration = 5f; // Durée de la douche en secondes

    private Coroutine showerCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Si une coroutine est déjà en cours, on la stop pour redémarrer le timer
            if (showerCoroutine != null)
            {
                StopCoroutine(showerCoroutine);
            }

            // On lance la coroutine qui gère la douche
            showerCoroutine = StartCoroutine(StartShowerTimer());
        }
    }

    private IEnumerator StartShowerTimer()
    {
        // Si les particules ne tournent pas déjà, on les démarre
        if (!showerParticles.isPlaying)
            showerParticles.Play();

        // Attend la durée de la douche
        yield return new WaitForSeconds(showerDuration);

        // Stoppe les particules
        showerParticles.Stop();
        showerCoroutine = null; // Reset la coroutine
    }
}
