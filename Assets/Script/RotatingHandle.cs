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
    public float showerDuration = 5f;
    private float showerTimer = 0f;
    private bool showerActive = false;
    
    [Header("Sound")]
    public AudioSource audioSource;
    
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
            // Arrêter la douche et le son
            StopShower();
        }
        else if (finalAngle < 0f)
        {
            float coldPercent = Mathf.InverseLerp(0f, -180f, finalAngle);
            float temperature = Mathf.Lerp(20f, 5f, coldPercent);
            Debug.Log($"Eau froide – Température : {temperature:F1}°C");
            
            ActivateShower();
        }
        else if (finalAngle > 0f)
        {
            float hotPercent = Mathf.InverseLerp(0f, 180f, finalAngle);
            float temperature = Mathf.Lerp(20f, 60f, hotPercent);
            Debug.Log($"Eau chaude – Température : {temperature:F1}°C");
            
            ActivateShower();
        }
    }
    
    void ActivateShower()
    {
        if (!showerParticles.isPlaying)
            showerParticles.Play();
        
        // Démarrer le son de douche
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
        showerTimer = showerDuration;
        showerActive = true;
    }
    
    void StopShower()
    {
        if (showerParticles.isPlaying)
            showerParticles.Stop();
        
        // Arrêter le son
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        showerActive = false;
    }
    
    void Update()
    {
        // Gestion de la rotation du bouton
        if (isGrabbed && interactor != null)
        {
            Quaternion delta = interactor.transform.rotation * Quaternion.Inverse(startHandRotation);
            float deltaZ = delta.eulerAngles.z;
            if (deltaZ > 180f) deltaZ -= 360f;
            
            float newAngle = startAngle + deltaZ * rotationSpeed;
            Vector3 euler = transform.localEulerAngles;
            euler.z = newAngle;
            transform.localEulerAngles = euler;
        }
        
        // Gestion du timer de la douche
        if (showerActive)
        {
            showerTimer -= Time.deltaTime;
            
            if (showerTimer <= 0f)
            {
                StopShower();
                Debug.Log("Douche éteinte automatiquement");
            }
        }
    }
    
    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}