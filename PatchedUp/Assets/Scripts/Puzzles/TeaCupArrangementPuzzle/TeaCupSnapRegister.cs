using System;
using UnityEngine;

public class TeaCupSnapRegister : MonoBehaviour
{
    private Transform saucerLocation;
    [SerializeField] private int cupId;
    [SerializeField] private float repickDistance = 0.3f;
    private bool isSnapped = false;
    private Collider snappedCup;

    private void Start()
    {
        saucerLocation = this.transform;
    }
    
    private void OnTriggerEnter(Collider other) => TrySnap(other);
    private void OnTriggerStay(Collider other) => TrySnap(other);

    private void TrySnap(Collider other)
    {
        if (isSnapped || !other.CompareTag("TeaCup")) return;
        
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null || rb.isKinematic) return;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.transform.position = saucerLocation.position;
        rb.transform.rotation = saucerLocation.rotation;

        isSnapped = true;
        snappedCup = other;
        TeaCupPuzzleEvents.OnSnapEvent(true, cupId);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isSnapped || other != snappedCup) return;
        
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        Vector3 itemPos = rb != null ? rb.position : other.transform.position;
        if (Vector3.Distance(itemPos, saucerLocation.position) < repickDistance) return;

        isSnapped = false;
        snappedCup = null;
        TeaCupPuzzleEvents.OnSnapEvent(false, cupId);
    }
    
    public bool GetIsSnapped()
    {
        return isSnapped;
    }
    
    
}
