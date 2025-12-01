using UnityEngine;
using TMPro;

public class PenguinShowerDetector : MonoBehaviour
{
    [Header("Shower Settings")]
    public float requiredWetTime = 30f;
    public ParticleSystem showerParticles;

    [Header("Progression")]
    public float wetProgress = 0f;

    [Header("UI Rinçage")]
    public TextMeshProUGUI rinseProgressText;   // <<< AJOUT

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    private bool hasPlayedSuccess = false;

    private bool isUnderShower = false;
    private float timeUnderShower = 0f;
    private float nextProgressLog = 0f;

    private int initialFoamCount = -1;

    void Update()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        wetProgress = Mathf.Clamp01(timeUnderShower / requiredWetTime);

        // Log chaque seconde
        if (timeUnderShower >= nextProgressLog)
        {
            Debug.Log($"Mouillage : {(wetProgress * 100f):F0}%");
            nextProgressLog += 1f;
        }

        // Initialisation du nombre total de bulles
        if (initialFoamCount == -1)
        {
            initialFoamCount = SoapWashDetector_Simple.foamInstances.Count;
            UpdateRinseProgressUI(); // affiche "0 %"
        }

        // Suppression progressive proportionnelle
        RemoveFoamProportionally();

        // Reset pour la prochaine frame
        isUnderShower = false;
    }


    private void RemoveFoamProportionally()
    {
        if (initialFoamCount <= 0)
            return;

        int bubblesToKeep = Mathf.RoundToInt(initialFoamCount * (1f - wetProgress));

        int currentCount = SoapWashDetector_Simple.foamInstances.Count;

        while (currentCount > bubblesToKeep)
        {
            GameObject foam = SoapWashDetector_Simple.foamInstances[currentCount - 1];

            if (foam != null)
                GameObject.Destroy(foam);

            SoapWashDetector_Simple.foamInstances.RemoveAt(currentCount - 1);
            currentCount--;
        }

        UpdateRinseProgressUI();  // <<< AJOUT : mettre à jour l’UI à chaque changement
    }


    private void UpdateRinseProgressUI()
    {
        if (rinseProgressText == null || initialFoamCount <= 0)
            return;

        int remaining = SoapWashDetector_Simple.foamInstances.Count;
        int removed = initialFoamCount - remaining;

        // 🚫 Aucune bulle retirée = 0%
        if (removed <= 0)
        {
            rinseProgressText.text = "0 %";
            return;
        }

        float progress = Mathf.Clamp01((float)removed / initialFoamCount);
        int percent = Mathf.RoundToInt(progress * 100f);

        rinseProgressText.text = percent + " %";

        // 🎉 Succès : toutes les bulles sont retirées
        if (!hasPlayedSuccess && percent >= 100)
        {
            hasPlayedSuccess = true;

            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            Debug.Log("🎉 Succès : Pingouin totalement rincé !");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other == showerParticles.gameObject)
            isUnderShower = true;
    }

    private void OnParticleTrigger()
    {
        isUnderShower = true;
    }
}