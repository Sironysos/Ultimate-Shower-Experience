using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KnobTranslator : MonoBehaviour
{
    public float translationSpeed = 0.05f;
    public float angleSensitivity = 0.2f;

    public float minX = -0.1f;   // limite gauche
    public float maxX = 0.1f;   // limite droite

    private bool isGrabbed = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    private float startHandYaw;

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;

        if (interactor != null)
        {
            startHandYaw = interactor.transform.eulerAngles.y;
        }
    }

    public void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactor = null;
    }

    void Update()
    {
        if (isGrabbed && interactor != null)
        {
            float currentYaw = interactor.transform.eulerAngles.y;
            float deltaYaw = Mathf.DeltaAngle(startHandYaw, currentYaw);

            float direction = deltaYaw * angleSensitivity;

            // Translation locale
            transform.Translate(Vector3.right * direction * translationSpeed * Time.deltaTime, Space.Self);

            // Récupère la position et clamp sur X
            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.localPosition = pos;
        }
    }
}
