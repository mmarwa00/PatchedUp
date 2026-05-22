using System;
using UnityEngine;

public class TeaCupSnapRegister : MonoBehaviour
{
    private Transform saucerLocation;
    [SerializeField] private int cupId;
    private bool isSnapped = false;

    private void Start()
    {
        saucerLocation = this.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TeaCup"))
        {
            var go = other.gameObject;
            go.transform.position = saucerLocation.position;
            go.transform.rotation = saucerLocation.rotation;
            isSnapped = true;
            TeaCupPuzzleEvents.OnSnapEvent(isSnapped, cupId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TeaCup"))
        {
            isSnapped = false;
            TeaCupPuzzleEvents.OnSnapEvent(isSnapped, cupId);
        }
    }
    
    public bool GetIsSnapped()
    {
        return isSnapped;
    }
    
    
}
