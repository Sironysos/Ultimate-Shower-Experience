using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class TowelDryDetector_Rigid : MonoBehaviour
{
    [Header("Réglages du frottement")]
    public float minVelocity = 0.4f;

    [Header("Temps pour sécher complètement")]
    public float requiredDryTime = 30f; // <<< AJOUT

    [Header("Progression")]
    public float dryingProgress = 0f; // 0 = trempé, 1 = sec

    [Header("UI Séchage")]
    public TextMeshProUGUI dryingProgressText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;

    private bool hasPlayedSuccess = false;

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isTouchingPenguin = false;

    void Start()
    {
        lastPos = transform.position;

        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
            grab.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        Vector3 velocity = (transform.position - lastPos) / Time.deltaTime;

        // Séchage continu tant que la serviette bouge ET touche le pingouin
        if (isTouchingPenguin && velocity.magnitude > minVelocity)
        {
            dryingProgress += Time.deltaTime / requiredDryTime;
            dryingProgress = Mathf.Clamp01(dryingProgress);

            Debug.Log($"Séchage → {dryingProgress * 100f}%");
            UpdateDryingUI();
            CheckSuccessSound();
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
        UpdateDryingUI();
    }

    private void UpdateDryingUI()
    {
        if (dryingProgressText == null)
            return;

        int percent = Mathf.RoundToInt(dryingProgress * 100f);
        dryingProgressText.text = percent + " %";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Penguin"))
            isTouchingPenguin = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Penguin"))
            isTouchingPenguin = false;
    }

    private void CheckSuccessSound()
    {
        if (!hasPlayedSuccess && dryingProgress >= 1f)
        {
            hasPlayedSuccess = true;

            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            Debug.Log("🎉 Succès : Pingouin complètement sec !");
        }
    }
}