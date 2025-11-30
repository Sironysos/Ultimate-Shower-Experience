using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HairdryerSoundOnGrab : MonoBehaviour
{
    public AudioSource audioSource;
    
    [Header("Son du sèche-cheveux")]
    public AudioClip hairdryerSound;
    
    [Header("Paramètres audio")]
    [Range(0f, 1f)]
    public float volume = 0.7f;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
    void Start()
    {
        // Récupère le composant XRGrabInteractable
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable manquant sur " + gameObject.name);
            return;
        }
        
        // Configure ou crée l'AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configuration pour VR
        audioSource.playOnAwake = false;
        audioSource.loop = true; // Important : le son tourne en boucle
        audioSource.spatialBlend = 1f; // Son 3D
        audioSource.spatialize = true;
        audioSource.minDistance = 0.5f;
        audioSource.maxDistance = 15f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = volume;
        audioSource.clip = hairdryerSound;
        
        // Abonne-toi aux événements de grab
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }
    
    void OnGrab(SelectEnterEventArgs args)
    {
        // Joue le son quand on grab
        if (hairdryerSound != null && audioSource != null)
        {
            audioSource.Play();
        }
    }
    
    void OnRelease(SelectExitEventArgs args)
    {
        // Arrête le son quand on relâche
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    void OnDestroy()
    {
        // Nettoie les listeners
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}