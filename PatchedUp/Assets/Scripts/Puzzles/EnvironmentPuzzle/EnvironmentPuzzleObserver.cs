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
        EnvironmentPuzzleEvents.OnToggleLights += HandleLighting;
    }

    private void OnDisable()
    {
        EnvironmentPuzzleEvents.OnEnvironmentSound -= HandleEnvironmentSound;
        EnvironmentPuzzleEvents.OnToggleLights -= HandleLighting;
    }

    private void HandleEnvironmentSound(AudioClip clip, AudioMixerGroup audioGroup, Vector3 position)
    {
        audioSource.outputAudioMixerGroup = audioGroup;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void HandleLighting(Light lamp, bool on)
    {
        lamp.enabled = !on;
    }
}

