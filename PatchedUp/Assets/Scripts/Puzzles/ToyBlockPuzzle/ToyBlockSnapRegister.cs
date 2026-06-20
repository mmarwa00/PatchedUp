namespace Puzzles.ToyBlockPuzzle
{
    using System;
    using UnityEngine;

    public class ToyBlockSnapRegister : MonoBehaviour
    {
        private Transform placementLocation;
        [SerializeField] private int locationId;
        [SerializeField] private char designatedLetter;
        private bool isSnapped = false;

        private void Start()
        {
            placementLocation = this.transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ToyBlock"))
            {
                var go = other.gameObject;
                go.transform.position = placementLocation.position;
                go.transform.rotation = placementLocation.rotation;
                
                ToyBlock toy = go.GetComponent<ToyBlock>();
                char blockLetter = toy.GetBlockLetter();
                isSnapped = true;
                ToyBlockPuzzleEvents.BlockPlacedEvent(isSnapped, designatedLetter, blockLetter, locationId);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ToyBlock"))
            {
                var go = other.gameObject;
                ToyBlock toy = go.GetComponent<ToyBlock>();
                char blockLetter = toy.GetBlockLetter();
                
                isSnapped = false;
                ToyBlockPuzzleEvents.BlockPlacedEvent(isSnapped, designatedLetter, blockLetter, locationId);
            }
        }
    
        public bool GetIsSnapped()
        {
            return isSnapped;
        }
    
    
    }
}
