using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentPuzzleObserver : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EnvironmentPuzzleEvents.OnEnvironmentSound += HandleEnvironmentSound;
    }

    private void OnDisable()
    {
        EnvironmentPuzzleEvents.OnEnvironmentSound -= HandleEnvironmentSound;
    }

    private void HandleEnvironmentSound(AudioClip clip, AudioMixerGroup audioGroup, Vector3 position)
    {
        
    }
}

