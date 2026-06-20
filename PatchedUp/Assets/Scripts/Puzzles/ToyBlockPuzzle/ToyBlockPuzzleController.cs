using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Puzzles.ToyBlockPuzzle
{
    public class ToyBlockPuzzleController : MonoBehaviour
    {
        [Header("Puzzle Settings")]
        [SerializeField] private ToyBlocks toyBlocks;
        [SerializeField] private char[] wordToWrite;
        
        private readonly Dictionary<int, char> placedLetters = new Dictionary<int, char>();
        private bool solved;

        [Header("Audio")]
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioClip failAudioClip;
        private AudioSource audioSource;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private void Start()
        {
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
            if (solved) return;
            
            if (isPlaced) placedLetters[id] = placedLetter;
            else placedLetters.Remove(id);
            
            if (CountCorrect() == wordToWrite.Length)
            {
                OnPuzzleCompleted();
                return;
            }
            
            if (isPlaced && placedLetters.Count == wordToWrite.Length)
            {
                audioSource.PlayOneShot(failAudioClip);
            }
        }

        private int CountCorrect()
        {
            int correct = 0;
            foreach (KeyValuePair<int, char> slot in placedLetters)
            {
                if (slot.Key >= 0 && slot.Key < wordToWrite.Length && slot.Value == wordToWrite[slot.Key])
                    correct++;
            }
            return correct;
        }

        private void OnPuzzleCompleted()
        {
            if (solved) return;
            solved = true;

            audioSource.PlayOneShot(audioClip);
            PuzzleManager.Instance.RegisterCompletedPuzzle(true);
        }
    }
}
