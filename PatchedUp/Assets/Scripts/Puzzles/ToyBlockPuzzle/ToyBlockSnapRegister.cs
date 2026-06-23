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

        // The slot self-heals: rather than relying on a single OnTriggerExit event (which could fire
        // while the block was still within repickDistance and strand the slot as permanently occupied),
        // we poll the snapped block here. A pinned block sits exactly at placementLocation, so it never
        // releases by accident; once it has been picked up and moved away, the slot frees reliably.
        private void Update()
        {
            if (!isSnapped) return;

            if (snappedBlock == null)
            {
                Release(default);
                return;
            }

            Rigidbody rb = snappedBlock.GetComponentInParent<Rigidbody>();
            Vector3 itemPos = rb != null ? rb.position : snappedBlock.transform.position;
            if (Vector3.Distance(itemPos, placementLocation.position) > repickDistance)
            {
                ToyBlock toy = snappedBlock.GetComponentInParent<ToyBlock>();
                Release(toy != null ? toy.GetBlockLetter() : default);
            }
        }

        private void Release(char blockLetter)
        {
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
