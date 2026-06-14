using UnityEngine;
using UnityEngine.Audio;

namespace Puzzles.ToyBlockPuzzle
{
    public class ToyBlockPuzzleController : MonoBehaviour
    {
        [Header("Puzzle Settings")]
        [SerializeField] private ToyBlocks toyBlocks;
        [SerializeField] private char[] wordToWrite;
        private int placed;
        private int correctlyPlaced;
        
        [Header("Audio")]
        [SerializeField] private AudioClip audioClip;
        private AudioSource audioSource;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private void Start()
        {
            placed = 0;
            correctlyPlaced = 0;
            this.audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        private void OnEnable()
        {
            ToyBlockPuzzleEvents.OnBlockPlaced += HandlePlacedToyBlocks;
        }

        private void OnDisable()
        {
            ToyBlockPuzzleEvents.OnBlockPlaced -= HandlePlacedToyBlocks;
        }

        private void HandlePlacedToyBlocks(bool isPlaced, char designatedLetter, char placedLetter, int id)
        {

            if (isPlaced) { placed++; } else { placed--; }
            
            if (designatedLetter == placedLetter && placedLetter == wordToWrite[id])
            {
                correctlyPlaced++;
                if (correctlyPlaced == wordToWrite.Length)
                {
                    OnPuzzleCompleted();
                }
            } 
        }

        private void OnPuzzleCompleted()
        {
            audioSource.PlayOneShot(audioClip);
            PuzzleManager.Instance.RegisterCompletedPuzzle(true);
        }
    }
}