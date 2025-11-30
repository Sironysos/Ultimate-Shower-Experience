using UnityEngine;

public class PlantShowerDetector : MonoBehaviour
{
    [Header("Shower Settings")]
    public float requiredWetTime = 30f;       // Temps total pour être mouillé
    public ParticleSystem showerParticles;    // Référence au particle system

    [Header("Progression")]
    public float wetProgress = 0f;

    private bool isUnderShower = false;
    private float timeUnderShower = 0f;
    private float nextProgressLog = 0f;

    void Update()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        wetProgress = Mathf.Clamp01(timeUnderShower / requiredWetTime);

        // Log chaque seconde
        if (timeUnderShower >= nextProgressLog)
        {
            Debug.Log($"Plante mouillage : {(wetProgress * 100f):F0}%");
            nextProgressLog += 1f;
        }

        if (wetProgress >= 1f)
        {
            Debug.Log("Plante totalement arrosée !");
        }

        // IMPORTANT : reset l'état à chaque frame
        // il faudra recevoir une collision pour qu'il reste "sous la douche"
        isUnderShower = false;
    }

    // Détection des collisions de particules
    private void OnParticleCollision(GameObject other)
    {
        if (other == showerParticles.gameObject)
        {
            isUnderShower = true;
        }
    }

    private void OnParticleTrigger()
    {
        isUnderShower = true;
    }
}
