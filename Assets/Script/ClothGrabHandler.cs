using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClothGrabHandler : MonoBehaviour
{
    public Cloth clothSim;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        clothSim.enabled = false; 
    }

    void OnRelease(SelectExitEventArgs args)
    {
        clothSim.enabled = true; 
    }
}
