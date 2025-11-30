using UnityEngine;

public class PenguinShowerDetector : MonoBehaviour
{
    [Header("Shower Settings")]
    public float requiredWetTime = 30f;
    public ParticleSystem showerParticles;

    [Header("Progression")]
    public float wetProgress = 0f;

    private bool isUnderShower = false;
    private float timeUnderShower = 0f;
    private float nextProgressLog = 0f;

    private int initialFoamCount = -1;   // <- Stocke le nombre de bulles au début du rinçage


    void Update()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        wetProgress = Mathf.Clamp01(timeUnderShower / requiredWetTime);

        // Log 
        if (timeUnderShower >= nextProgressLog)
        {
            Debug.Log($"Mouillage : {(wetProgress * 100f):F0}%");
            nextProgressLog += 1f;
        }

        // Enregistrer le nombre initial de bulles au tout début du rinçage
        if (initialFoamCount == -1)
        {
            initialFoamCount = SoapWashDetector_Simple.foamInstances.Count;
        }

        // --- SUPPRESSION PROPORTIONNELLE ---
        RemoveFoamProportionally();

        // Reset shower flag
        isUnderShower = false;
    }



    private void RemoveFoamProportionally()
    {
        if (initialFoamCount <= 0)
            return;

        // combien de bulles doivent RESTER ?
        int bubblesToKeep = Mathf.RoundToInt(initialFoamCount * (1f - wetProgress));

        // combien de bulles nous avons actuellement ?
        int currentCount = SoapWashDetector_Simple.foamInstances.Count;

        // s’il y en a trop, on en supprime
        while (currentCount > bubblesToKeep)
        {
            GameObject foam = SoapWashDetector_Simple.foamInstances[currentCount - 1];

            if (foam != null)
                GameObject.Destroy(foam);

            SoapWashDetector_Simple.foamInstances.RemoveAt(currentCount - 1);

            currentCount--;
        }
    }



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
