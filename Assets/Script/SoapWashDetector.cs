using UnityEngine;
using System.Collections.Generic; // IMPORTANT pour List<T>

public class SoapWashDetector_Simple : MonoBehaviour
{
    [Header("Réglages du frottement")]
    public float minVelocity = 0.2f;
    public float minDirectionChange = 0.15f;
    public float washScorePerSwipe = 0.2f;

    [Header("Progression")]
    public float washProgress = 0f;

    [Header("Effets temporaires (Bulles du savon)")]
    public ParticleSystem bubbleParticles;

    [Header("Mousse persistante")]
    public GameObject foamPrefab;                // <<< manquait dans ton code !
    public Transform penguinRoot;
    public float foamSpawnRate = 0.05f;
    private float lastFoamTime = 0f;

    // Liste globale des bulles créées
    public static List<GameObject> foamInstances = new List<GameObject>();

    [Header("Taille de la mousse (aléatoire)")]
    public float foamMinScale = 0.02f;
    public float foamMaxScale = 0.06f;

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isHeld = false;

    private bool isTouchingPenguin = false;


    void Start()
    {
        lastPos = transform.position;
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grab.selectEntered.AddListener((args) =>
        {
            isHeld = true;
            lastPos = transform.position;
            lastVelocity = Vector3.zero;
        });

        grab.selectExited.AddListener((args) =>
        {
            isHeld = false;
            washProgress = Mathf.Clamp01(washProgress);

            Debug.Log($"[SAVON RELÂCHÉ] Savonnage = {washProgress * 100f}%");

            if (bubbleParticles != null)
                bubbleParticles.Stop();
        });
    }


    void Update()
    {
        if (!isHeld)
            return;

        Vector3 velocity = (transform.position - lastPos) / Time.deltaTime;

        if (velocity.magnitude > minVelocity)
        {
            float dirDot = Vector3.Dot(velocity.normalized, lastVelocity.normalized);

            if (dirDot < -minDirectionChange)
            {
                if (isTouchingPenguin)
                {
                    washProgress = Mathf.Clamp01(washProgress + washScorePerSwipe);

                    if (bubbleParticles != null && !bubbleParticles.isPlaying)
                        bubbleParticles.Play();

                    // --- Spawn mousse persistante ---
                    TrySpawnFoam();
                }
            }
        }
        else
        {
            if (bubbleParticles != null && bubbleParticles.isPlaying && !isTouchingPenguin)
                bubbleParticles.Stop();
        }

        lastVelocity = velocity;
        lastPos = transform.position;
    }


    // --- Vérifie si on peut spawn une nouvelle bulle ---
    private void TrySpawnFoam()
    {
        if (Time.time - lastFoamTime < foamSpawnRate)
            return;

        SpawnFoam();
        lastFoamTime = Time.time;
    }


    // --- Crée une bulle persistante collée au pingouin ---
    private void SpawnFoam()
    {
        if (foamPrefab == null || penguinRoot == null)
            return;

        Vector3 spawnPos = transform.position;

        GameObject foam = Instantiate(foamPrefab, spawnPos, Quaternion.identity);

        // Taille aléatoire
        float randomScale = Random.Range(foamMinScale, foamMaxScale);
        foam.transform.localScale = Vector3.one * randomScale;

        // La mousse reste collée au pingouin
        foam.transform.SetParent(penguinRoot, true);

        // On l’ajoute à la liste pour pouvoir la supprimer sous la douche
        foamInstances.Add(foam);
    }


    // ---- Détection de contact avec le pingouin ----
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            isTouchingPenguin = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            isTouchingPenguin = false;

            if (bubbleParticles != null)
                bubbleParticles.Stop();
        }
    }
}
