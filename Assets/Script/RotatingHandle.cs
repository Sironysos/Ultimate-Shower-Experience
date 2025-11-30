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
    
    [Header("Shower Control")]
    public ParticleSystem showerParticles;
    public float showerDuration = 5f;
    private float showerTimer = 0f;
    private bool showerActive = false;
    
    [Header("Sound")]
    public AudioSource audioSource;
    
    void Start()
    {
        // Configure l'AudioSource si assigné
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // Son 3D
            audioSource.spatialize = true;
            audioSource.minDistance = 0.1f;
            audioSource.maxDistance = 50f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.volume = 0.1f;
        }
    }
    
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
        
        // Gestion de la douche selon la température
        float finalAngle = NormalizeAngle(transform.localEulerAngles.z);
        
        if (finalAngle >= -30f && finalAngle <= 30f)
        {
            Debug.Log($"Température réglée : {currentTemperature:F1}°C - Éteint");
            StopShower();
        }
        else
        {
            Debug.Log($"Température réglée : {currentTemperature:F1}°C");
            ActivateShower();
        }
    }
    
    void ActivateShower()
    {
        if (showerParticles != null && !showerParticles.isPlaying)
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
        if (showerParticles != null && showerParticles.isPlaying)
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
            
            UpdateTemperature(newAngle);
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

        UpdateParticleColor();
    }
    
    float NormalizeAngle(float a)
    {
        if (a > 180) a -= 360;
        return a;
    }

    void UpdateParticleColor()
    {
        if (showerParticles == null) return;

        var main = showerParticles.main;

        if (currentTemperature <= 10f)
        {
            main.startColor = Color.blue;           // Froid
        }
        else if (currentTemperature <= 30f)
        {
            main.startColor = Color.white;         // tiède
        }
        else
        {
            main.startColor = Color.red;           // Chaud
        }
    }
}