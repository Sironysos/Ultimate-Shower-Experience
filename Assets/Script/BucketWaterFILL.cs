using UnityEngine;

public class BucketWaterFill : MonoBehaviour
{
    [Header("Shower Settings")]
    public ParticleSystem showerParticles;
    public float timeToFill = 10f;  // Temps total pour remplir complètement

    [Header("Meshes du seau")]
    public GameObject[] bucketLevels;
    // Ordre : 0 = vide, 1 = peu d'eau, 2 = moyen, 3 = plein

    private float fillProgress = 0f;
    private float timeUnderShower = 0f;
    private bool isUnderShower = false;

    void Start()
    {
        SetBucketLevel(0); // Commencer vide
    }

    void Update()
    {
        if (!isUnderShower)
            return;

        timeUnderShower += Time.deltaTime;
        fillProgress = Mathf.Clamp01(timeUnderShower / timeToFill);

        UpdateBucketMesh();

        // Reset à chaque frame — doit être remis à true par OnParticleCollision
        isUnderShower = false;
    }

    // Détection des particules
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

    // ----- gestion des levels -----

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
