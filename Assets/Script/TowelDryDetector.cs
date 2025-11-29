using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TowelDryDetector_Rigid : MonoBehaviour
{
    [Header("Réglages du frottement")]
    public float minVelocity = 0.4f;
    public float minDirectionChange = 0.25f;
    public float dryScorePerSwipe = 0.3f;

    [Header("Progression")]
    public float dryingProgress = 0f; // 0 = trempé, 1 = sec

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    // ---- NOUVEAU ----
    private bool isTouchingPenguin = false;

    void Start()
    {
        lastPos = transform.position;

        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
            grab.selectExited.AddListener(OnRelease);
        else
            Debug.LogWarning("Aucun XRGrabInteractable trouvé !");
    }

    void Update()
    {
        Vector3 velocity = (transform.position - lastPos) / Time.deltaTime;

        if (velocity.magnitude > minVelocity)
        {
            float dirDot = Vector3.Dot(velocity.normalized, lastVelocity.normalized);

            // Détection du va-et-vient
            if (dirDot < -minDirectionChange)
            {
                // ---- CONDITION IMPORTANTE ----
                if (isTouchingPenguin)
                {
                    dryingProgress = Mathf.Clamp01(dryingProgress + dryScorePerSwipe);

                    Debug.Log($"Séchage → {dryingProgress * 100f}%");
                }
            }
        }

        lastVelocity = velocity;
        lastPos = transform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log($"[SERVIETTE RELÂCHÉE] Séchage total : {dryingProgress * 100f}%");
    }

    public void ResetDrying()
    {
        dryingProgress = 0f;
    }

    // -------- DÉTECTION DE CONTACT --------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            isTouchingPenguin = true;
            Debug.Log("Serviette en contact avec le pingouin.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            isTouchingPenguin = false;
            Debug.Log("Serviette n'est plus en contact avec le pingouin.");
        }
    }
}
