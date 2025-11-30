using UnityEngine;
using System.Collections.Generic;

public class VRHeadNoGoZonesRobust : MonoBehaviour
{
    [Header("Références")]
    public Transform xrOrigin;                 // XR Origin à déplacer
    public List<Collider> forbiddenZones;      // Colliders représentant les zones interdites

    [Header("Réglages")]
    public float pushDistance = 0.25f;         // Distance de repoussement
    public float smoothTime = 0.08f;           // Douceur du mouvement

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetXROriginPos;
    private bool isPushing = false;

    private void Update()
    {
        if (!isPushing) return;

        Vector3 current = xrOrigin.position;
        Vector3 target = new Vector3(targetXROriginPos.x, current.y, targetXROriginPos.z);
        xrOrigin.position = Vector3.SmoothDamp(current, target, ref velocity, smoothTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!forbiddenZones.Contains(other))
            return;

        // Point le plus proche sur le collider
        Vector3 closestPoint = other.ClosestPoint(transform.position);

        // Direction horizontale vers l'extérieur
        Vector3 pushDir = (transform.position - closestPoint);
        pushDir.y = 0f;
        if (pushDir.sqrMagnitude < 0.0001f) return; // éviter division par zéro
        pushDir.Normalize();

        targetXROriginPos = xrOrigin.position + pushDir * pushDistance;
        isPushing = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!forbiddenZones.Contains(other))
            return;

        isPushing = false;
        velocity = Vector3.zero;
    }
}
