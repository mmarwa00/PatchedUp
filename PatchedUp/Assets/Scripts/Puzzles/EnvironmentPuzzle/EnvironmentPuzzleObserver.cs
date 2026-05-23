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
        EnvironmentPuzzleEvents.OnPuzzleSuccess += PlayPuzzleSuccessSound;
    }

    private void OnDisable()
    {
        EnvironmentPuzzleEvents.OnEnvironmentSound -= HandleEnvironmentSound;
        EnvironmentPuzzleEvents.OnToggleLights -= HandleLighting;
        EnvironmentPuzzleEvents.OnEnvironmentParticles -= HandleParticles;
    }

    private void HandleEnvironmentSound(AudioClip clip, AudioMixerGroup audioGroup, Vector3 position, bool isPlaying)
    {
        if (isPlaying)
        {
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = audioGroup;
            audioSource.Stop();
        }
        else
        {
            audioSource.transform.position = position;
            audioSource.outputAudioMixerGroup = audioGroup;
            audioSource.clip = clip;
            audioSource.Play(); 
        }
        
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
            particles.Clear();
        }
        else
        {
            particles.Play();
        }
        
    }
    
    private void PlayPuzzleSuccessSound(AudioClip clip, AudioMixerGroup audioMixerGroup,  Vector3 position)
    {
        audioSource.transform.position = position;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.PlayOneShot(clip);
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

