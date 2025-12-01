using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class KnobTranslator : MonoBehaviour
{
    public float translationSpeed = 0.05f;
    public float angleSensitivity = 0.2f;

    [Header("Limites de translation")]
    public float minX = -0.1f;
    public float maxX = 0.1f;

    [Header("UI Progression")]
    public TextMeshProUGUI progressText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    private bool hasPlayedSuccess = false;

    private bool isGrabbed = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    private float startHandYaw;

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;

        if (interactor != null)
            startHandYaw = interactor.transform.eulerAngles.y;
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

            transform.Translate(Vector3.right * direction * translationSpeed * Time.deltaTime, Space.Self);

            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.localPosition = pos;
        }

        // --- Mise à jour progression UI ---
        float progress = Mathf.InverseLerp(minX, maxX, transform.localPosition.x);

        if (progressText != null)
            progressText.text = $"{Mathf.RoundToInt(progress * 100f)} %";

        // --- Succès ---
        if (!hasPlayedSuccess && progress >= 1f)
        {
            hasPlayedSuccess = true;

            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            Debug.Log("🎉 Succès : Locket complètement fermé !");
        }
    }
}
