using UnityEngine;
using TMPro;

public class BucketWaterFillAndDrain : MonoBehaviour
{
    [Header("Shower Settings")]
    public ParticleSystem showerParticles;
    public float timeToFill = 10f;

    [Header("Vidage du seau")]
    public float minTiltAngleToDrain = 40f;
    public float maxTiltAngle = 120f;
    public float minDrainSpeed = 0.2f;
    public float maxDrainSpeed = 2f;
    public ParticleSystem drainParticles;

    [Header("Meshes du seau")]
    public GameObject[] bucketLevels;

    [Header("UI Progression")]
    public TextMeshProUGUI progressText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    private bool hasPlayedSuccess = false;

    private float fillProgress = 0f;        
    private float timeUnderShower = 0f;
    private bool isUnderShower = false;

    void Start()
    {
        SetBucketLevel(0); 
    }

    void Update()
    {
        HandleFill();
        HandleDrain();
        UpdateBucketMesh();
        UpdateUIProgress();

        isUnderShower = false;
    }

    // ---------------------- UI UPDATE ----------------------
    private void UpdateUIProgress()
    {
        if (progressText == null)
            return;

        int percent = Mathf.RoundToInt(fillProgress * 100f);
        progressText.text = percent + " %";

        // 🎉 Succès : seau totalement rempli
        if (!hasPlayedSuccess && fillProgress >= 1f)
        {
            hasPlayedSuccess = true;

            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            Debug.Log("🎉 Succès : Seau complètement rempli !");
        }
    }

    // ---------------------- REMPLISSAGE ----------------------
    private void HandleFill()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        fillProgress = Mathf.Clamp01(timeUnderShower / timeToFill);
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

    // ---------------------- VIDAGE ----------------------
    private void HandleDrain()
    {
        if (fillProgress <= 0f)
        {
            if (drainParticles != null && drainParticles.isPlaying)
                drainParticles.Stop();
            return;
        }

        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);

        if (tiltAngle < minTiltAngleToDrain)
        {
            if (drainParticles != null && drainParticles.isPlaying)
                drainParticles.Stop();
            return;
        }

        float t = Mathf.InverseLerp(minTiltAngleToDrain, maxTiltAngle, tiltAngle);
        float drainSpeed = Mathf.Lerp(minDrainSpeed, maxDrainSpeed, t);

        fillProgress -= drainSpeed * Time.deltaTime;
        fillProgress = Mathf.Clamp01(fillProgress);

        if (drainParticles != null && !drainParticles.isPlaying)
            drainParticles.Play();
    }

    // ---------------------- MESH UPDATE ----------------------
    private void UpdateBucketMesh()
    {
        int level = Mathf.FloorToInt(fillProgress * (bucketLevels.Length - 1));
        SetBucketLevel(level);
    }

    private void SetBucketLevel(int level)
    {
        for (int i = 0; i < bucketLevels.Length; i++)
            bucketLevels[i].SetActive(i == level);
    }
}