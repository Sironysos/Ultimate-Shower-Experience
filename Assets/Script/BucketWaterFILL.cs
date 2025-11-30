using UnityEngine;

public class BucketWaterFillAndDrain : MonoBehaviour
{
    [Header("Shower Settings")]
    public ParticleSystem showerParticles;
    public float timeToFill = 10f;              // Temps pour remplir complètement

    [Header("Vidage du seau")]
    public float minTiltAngleToDrain = 40f;     // Angle minimum pour commencer à vider
    public float maxTiltAngle = 120f;           // Angle où ça coule très vite
    public float minDrainSpeed = 0.2f;          // Vitesse minimale de vidage
    public float maxDrainSpeed = 2f;            // Vitesse maximale de vidage
    public ParticleSystem drainParticles;       // Particules quand le seau se vide

    [Header("Meshes du seau")]
    public GameObject[] bucketLevels;           // 0 = vide, ... = plein

    private float fillProgress = 0f;            // 0 à 1
    private float timeUnderShower = 0f;
    private bool isUnderShower = false;

    void Start()
    {
        SetBucketLevel(0);  // Commencer vide
    }

    void Update()
    {
        HandleFill();
        HandleDrain();
        UpdateBucketMesh();

        // Reset shower detection (doit être remis à true via OnParticleCollision)
        isUnderShower = false;
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
        {
            isUnderShower = true;
        }
    }

    private void OnParticleTrigger()
    {
        isUnderShower = true;
    }

    // ---------------------- VIDAGE ----------------------

    private void HandleDrain()
    {
        // Si seau déjà vide ? ne rien faire
        if (fillProgress <= 0f)
        {
            if (drainParticles != null && drainParticles.isPlaying)
                drainParticles.Stop();
            return;
        }

        // Calcul de l'angle d'inclinaison
        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);

        // Pas assez incliné ? pas de vidage
        if (tiltAngle < minTiltAngleToDrain)
        {
            if (drainParticles != null && drainParticles.isPlaying)
                drainParticles.Stop();
            return;
        }

        // Calcul du facteur de vidage basé sur l'angle
        float t = Mathf.InverseLerp(minTiltAngleToDrain, maxTiltAngle, tiltAngle);
        float drainSpeed = Mathf.Lerp(minDrainSpeed, maxDrainSpeed, t);

        fillProgress -= drainSpeed * Time.deltaTime;
        fillProgress = Mathf.Clamp01(fillProgress);

        // Jouer particules d'écoulement
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
