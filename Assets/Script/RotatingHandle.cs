using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KnobRotator : MonoBehaviour
{
    public float rotationSpeed = 1f;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    private bool isGrabbed = false;

    private Quaternion startHandRotation;
    private float startAngle;

    [Header("Shower Control")]
    public ParticleSystem showerParticles;

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

        float finalAngle = NormalizeAngle(transform.localEulerAngles.z);

        // ------ LOGIC ------
        if (finalAngle >= -30f && finalAngle <= 30f)
        {
            Debug.Log("Éteint");
            if (showerParticles.isPlaying)
                showerParticles.Stop();
        }
        else if (finalAngle < 0f)
        {
            float coldPercent = Mathf.InverseLerp(0f, -180f, finalAngle);
            float temperature = Mathf.Lerp(20f, 5f, coldPercent);
            Debug.Log($"Eau froide — Température : {temperature:F1}°C");

            if (!showerParticles.isPlaying)
                showerParticles.Play();
        }
        else if (finalAngle > 0f)
        {
            float hotPercent = Mathf.InverseLerp(0f, 180f, finalAngle);
            float temperature = Mathf.Lerp(20f, 60f, hotPercent);
            Debug.Log($"Eau chaude — Température : {temperature:F1}°C");

            if (!showerParticles.isPlaying)
                showerParticles.Play();
        }
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
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
