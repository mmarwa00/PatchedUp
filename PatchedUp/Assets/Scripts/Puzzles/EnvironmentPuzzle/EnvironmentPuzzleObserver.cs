using System.Collections;
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
        EnvironmentPuzzleEvents.OnEnvironmentParticles += HandleParticles;
    }

    private void OnDisable()
    {
        EnvironmentPuzzleEvents.OnEnvironmentSound -= HandleEnvironmentSound;
        EnvironmentPuzzleEvents.OnToggleLights -= HandleLighting;
        EnvironmentPuzzleEvents.OnEnvironmentParticles -= HandleParticles;
    }

    private void HandleEnvironmentSound(AudioClip clip, AudioMixerGroup audioGroup, Vector3 position)
    {
        audioSource.outputAudioMixerGroup = audioGroup;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void HandleLighting(Light lamp, bool on,  float minIntensity, float maxIntensity)
    {
        if (on)
        {
            lamp.enabled = false;
            StopCoroutine(LightsFlicker(lamp, minIntensity, maxIntensity));
        }
        else
        {
            lamp.enabled = true;
            StartCoroutine(LightsFlicker(lamp, minIntensity, maxIntensity));
        }
    }

    private void HandleParticles(ParticleSystem particles, bool isOn)
    {
        if (isOn)
        {
            particles.Stop();
        }
        else
        {
            particles.Play();
        }
        
    }

    private IEnumerator LightsFlicker(Light lamp, float minIntensity, float maxIntensity)
    {
        while (lamp.enabled)
        {
            lamp.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForEndOfFrame();
        }
        
    }
}

