using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;

public class ShowerButton : MonoBehaviour
{
    [Header("Shower Settings")]
    public ParticleSystem showerParticles;
    public float activeDuration = 30f;   // durée max
    public bool useRandomDuration = true;

    private XRGrabInteractable grab;
    private XRBaseInteractor interactor;

    private Vector3 initialPos;
    private Quaternion initialRot;

    private Coroutine showerRoutine;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        // Pose de départ
        initialPos = transform.position;
        initialRot = transform.rotation;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        grab.trackPosition = false;
        grab.trackRotation = false;

        if (showerParticles != null)
            showerParticles.Stop();
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void LateUpdate()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as XRBaseInteractor;

        Debug.Log("hello");

        StartShower();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;

        transform.position = initialPos;
        transform.rotation = initialRot;
    }

    private void StartShower()
    {
        if (showerParticles == null)
        {
            Debug.LogWarning("Aucun ParticleSystem assigné !");
            return;
        }

        if (showerRoutine != null)
            StopCoroutine(showerRoutine);

        showerRoutine = StartCoroutine(ShowerTimer());
    }

    private IEnumerator ShowerTimer()
    {
        showerParticles.Play();

        float duration = activeDuration;

        // Durée aléatoire
        if (useRandomDuration)
        {
            duration = Random.Range(0f, activeDuration);
            Debug.Log($"Durée de douche aléatoire : {duration:F1} sec");
        }

        yield return new WaitForSeconds(duration);

        showerParticles.Stop();
        showerRoutine = null;

        Debug.Log("💧 Douche stoppée !");
    }
}
