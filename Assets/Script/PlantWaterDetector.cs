using UnityEngine;
using TMPro;

public class PlantShowerDetector : MonoBehaviour
{
    [Header("Shower Settings")]
    public float requiredWetTime = 30f;
    public ParticleSystem showerParticles;

    [Header("Progression")]
    public float wetProgress = 0f;

    [Header("UI de progression")]
    public TextMeshProUGUI progressText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    private bool hasPlayedSuccess = false;

    private bool isUnderShower = false;
    private float timeUnderShower = 0f;
    private float nextProgressLog = 0f;

    void Update()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        wetProgress = Mathf.Clamp01(timeUnderShower / requiredWetTime);

        UpdateUIProgress();
        CheckSuccessSound();   // <<< AJOUT

        if (timeUnderShower >= nextProgressLog)
        {
            Debug.Log($"Plante mouillage : {(wetProgress * 100f):F0}%");
            nextProgressLog += 1f;
        }

        if (wetProgress >= 1f)
        {
            Debug.Log("Plante totalement arrosée !");
        }

        isUnderShower = false;
    }

    private void UpdateUIProgress()
    {
        if (progressText == null)
            return;

        int percent = Mathf.RoundToInt(wetProgress * 100f);
        progressText.text = percent + " %";
    }

    private void CheckSuccessSound()
    {
        if (!hasPlayedSuccess && wetProgress >= 1f)
        {
            hasPlayedSuccess = true;

            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            Debug.Log("🎉Succès : Plante complètement arrosée !");
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