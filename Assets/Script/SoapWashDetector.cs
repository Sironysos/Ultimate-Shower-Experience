using UnityEngine;


public class SoapWashDetector_Simple : MonoBehaviour
{
    [Header("Réglages du frottement")]
    public float minVelocity = 0.2f;
    public float minDirectionChange = 0.15f;
    public float washScorePerSwipe = 0.2f;

    [Header("Progression")]
    public float washProgress = 0f;

    [Header("Effets")]
    public ParticleSystem bubbleParticles;   // optionnel

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isHeld = false;

    void Start()
    {
        lastPos = transform.position;

        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Quand on attrape le savon
        grab.selectEntered.AddListener((args) =>
        {
            isHeld = true;
            lastPos = transform.position;
            lastVelocity = Vector3.zero;
        });

        // Quand on lâche le savon
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

            // Détection du va-et-vient
            if (dirDot < -minDirectionChange)
            {
                washProgress = Mathf.Clamp01(washProgress + washScorePerSwipe);

                if (bubbleParticles != null && !bubbleParticles.isPlaying)
                    bubbleParticles.Play();
            }
        }
        else
        {
            if (bubbleParticles != null && bubbleParticles.isPlaying)
                bubbleParticles.Stop();
        }

        lastVelocity = velocity;
        lastPos = transform.position;
    }
}
