using UnityEngine;
using UnityEngine.Audio;

namespace Puzzles.KitchenTowerPuzzle
{
    public class KitchenTowerPuzzleController : MonoBehaviour
    {
        [SerializeField] private TowerController[] towerController;
        private int towerCount;
        private int finishedTowerCount = 0;
        
        [Header("Audio")]
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip failureClip;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        private AudioSource audioSource;


        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
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
                audioSource.clip = successClip;
                audioSource.Play();
                finishedTowerCount++;
            }
            else
            {
                audioSource.clip = failureClip;
                audioSource.Play();
            }
        }
    }
}