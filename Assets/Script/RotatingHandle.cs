using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KnobRotator : MonoBehaviour
{
    public float rotationSpeed = 1f;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    private bool isGrabbed = false;
    private Quaternion startHandRotation;
    private float startAngle;

    public float currentTemperature = 20f;

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        if (interactor == null) return;

        isGrabbed = true;
        startHandRotation = interactor.transform.rotation;
        startAngle = transform.localEulerAngles.z;
    }

    public void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactor = null;

        // ?? LOG TEMPERATURE LORS DU REL�CHEMENT
        Debug.Log($"Temp�rature r�gl�e : {currentTemperature:F1}�C");
    }

    void Update()
    {
        if (!isGrabbed || interactor == null) return;

        Quaternion delta = interactor.transform.rotation * Quaternion.Inverse(startHandRotation);
        float deltaZ = delta.eulerAngles.z;
        if (deltaZ > 180f) deltaZ -= 360f;

        float newAngle = startAngle + deltaZ * rotationSpeed;

        Vector3 euler = transform.localEulerAngles;
        euler.z = newAngle;
        transform.localEulerAngles = euler;

        UpdateTemperature(newAngle);
    }

    void UpdateTemperature(float angle)
    {
        float finalAngle = NormalizeAngle(angle);

        if (finalAngle >= -30 && finalAngle <= 30)
        {
            currentTemperature = 20f;  // neutre
        }
        else if (finalAngle < 0)
        {
            float t = Mathf.InverseLerp(0, -180, finalAngle);
            currentTemperature = Mathf.Lerp(20, 5, t); // Cold
        }
        else
        {
            float t = Mathf.InverseLerp(0, 180, finalAngle);
            currentTemperature = Mathf.Lerp(20, 60, t); // Hot
        }
    }

    float NormalizeAngle(float a)
    {
        if (a > 180) a -= 360;
        return a;
    }
}
