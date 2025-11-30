using UnityEngine;
using System.Collections.Generic;

public class SoapWashDetector_Simple : MonoBehaviour
{
    [Header("Réglages du frottement")]
    public float minVelocity = 0.2f;
    public float minDirectionChange = 0.15f;

    [Header("Progression (savonnage)")]
    public float washProgress = 0f;
    public int bubblesNeededForFullWash = 50;  // Nombre de bulles pour atteindre 100%

    [Header("Mousse persistante")]
    public GameObject foamPrefab;
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

    private BubbleSoundOnSpawnRandom bubbleSoundManager;

    void Start()
    {
        bubbleSoundManager = FindObjectOfType<BubbleSoundOnSpawnRandom>();

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
                    TrySpawnFoam();
                    UpdateWashProgress();
                }
            }
        }

        lastVelocity = velocity;
        lastPos = transform.position;
    }


    // Ajoute une nouvelle bulle au bon rythme
    private void TrySpawnFoam()
    {
        if (Time.time - lastFoamTime < foamSpawnRate)
            return;

        SpawnFoam();
        lastFoamTime = Time.time;
    }


    // Crée une bulle et l’attache au pingouin
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

        // On ajoute à la liste
        foamInstances.Add(foam);

        // ---- Joue le son de bulle ----
        if (bubbleSoundManager != null)
            bubbleSoundManager.PlayBubbleSound();
    }



    // Met à jour la progression selon le nombre de bulles créées
    private void UpdateWashProgress()
    {
        washProgress = Mathf.Clamp01((float)foamInstances.Count / bubblesNeededForFullWash);

        Debug.Log($"Savonnage : {(washProgress * 100f):F0}%");
    }


    // Détection contact pingouin
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
        }
    }
}
