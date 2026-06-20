namespace Puzzles.ToyBlockPuzzle
{
    using System;
    using UnityEngine;

    public class ToyBlockSnapRegister : MonoBehaviour
    {
        private Transform placementLocation;
        [SerializeField] private int locationId;
        [SerializeField] private char designatedLetter;
        [SerializeField] private float repickDistance = 0.3f;
        private bool isSnapped = false;
        private Collider snappedBlock;

        private void Start()
        {
            placementLocation = this.transform;
        }
        
        private void OnTriggerEnter(Collider other) => TrySnap(other);
        private void OnTriggerStay(Collider other) => TrySnap(other);

        private void TrySnap(Collider other)
        {
            if (isSnapped || !other.CompareTag("ToyBlock")) return;
            
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            if (rb == null || rb.isKinematic) return;

            ToyBlock toy = other.GetComponentInParent<ToyBlock>();
            if (toy == null) return;

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.transform.position = placementLocation.position;
            rb.transform.rotation = placementLocation.rotation;

            isSnapped = true;
            snappedBlock = other;
            ToyBlockPuzzleEvents.BlockPlacedEvent(true, designatedLetter, toy.GetBlockLetter(), locationId);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isSnapped || other != snappedBlock) return;
            
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            Vector3 itemPos = rb != null ? rb.position : other.transform.position;
            if (Vector3.Distance(itemPos, placementLocation.position) < repickDistance) return;

            ToyBlock toy = other.GetComponentInParent<ToyBlock>();
            char blockLetter = toy != null ? toy.GetBlockLetter() : default;

            isSnapped = false;
            snappedBlock = null;
            ToyBlockPuzzleEvents.BlockPlacedEvent(false, designatedLetter, blockLetter, locationId);
        }
    
        public bool GetIsSnapped()
        {
            return isSnapped;
        }
    
    
    }
}
