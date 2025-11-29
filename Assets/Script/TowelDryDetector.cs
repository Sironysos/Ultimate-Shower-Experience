using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TowelDryDetector_Rigid : MonoBehaviour
{
    public float minVelocity = 0.4f;
    public float minDirectionChange = 0.25f;
    public float dryScorePerSwipe = 0.3f;

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    public float dryingProgress = 0f; // 0 = trempé, 1 = sec

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Start()
    {
        lastPos = transform.position; // <-- le mesh rigide lui-même

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

            // Changement de direction = mouvement de frottement
            if (dirDot < -minDirectionChange)
            {
                dryingProgress = Mathf.Clamp01(dryingProgress + dryScorePerSwipe);
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
}
