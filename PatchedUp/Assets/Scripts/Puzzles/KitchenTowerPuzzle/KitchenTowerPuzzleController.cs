using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Puzzles.KitchenTowerPuzzle
{
    public class KitchenTowerPuzzleController : MonoBehaviour
    {
        [SerializeField] private TowerController[] towerController;
        private int towerCount;
        private readonly HashSet<int> solvedTowerIds = new HashSet<int>();

        [Header("Audio")]
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip failureClip;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        private AudioSource audioSource;

        [Header("Events")]
        [SerializeField] private UnityEvent onPuzzleSolved;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioMixerGroup != null) audioSource.outputAudioMixerGroup = audioMixerGroup;
            towerCount = towerController.Length;
        }

        private void OnEnable()
        {
            KitchenTowerEvents.OnTowerBuilt += HandleFinishedTower;
        }

        private void OnDisable()
        {
            KitchenTowerEvents.OnTowerBuilt -= HandleFinishedTower;
        }

        private void HandleFinishedTower(bool success, int towerId)
        {
            if (success)
            {
                if (successClip != null) audioSource.PlayOneShot(successClip);

                solvedTowerIds.Add(towerId);
                if (solvedTowerIds.Count == towerCount) HandlePuzzleSolved();
            }
            else
            {
                if (failureClip != null) audioSource.PlayOneShot(failureClip);
            }
        }

        private void HandlePuzzleSolved()
        {
            onPuzzleSolved?.Invoke();
            KitchenTowerEvents.OnPuzzleSolvedEvent();
        }
    }
}
