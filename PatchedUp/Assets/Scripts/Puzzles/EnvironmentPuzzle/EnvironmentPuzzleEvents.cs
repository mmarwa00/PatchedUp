using System;
using UnityEngine;
using UnityEngine.Audio;

public static class EnvironmentPuzzleEvents
{
    public static Action<Light, bool, float, float> OnToggleLights;

    public static Action<AudioClip, AudioMixerGroup, Vector3, bool> OnEnvironmentSound;
    
    public static Action<ParticleSystem, bool> OnEnvironmentParticles;

    public static Action<AudioClip, AudioMixerGroup, Vector3> OnPuzzleSuccess;

    public static void ToggleLights(Light light, bool on, float minIntensity, float maxIntensity)
    {
        OnToggleLights?.Invoke(light, on,  minIntensity, maxIntensity);
    }

    public static void EnvironmentSound(AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position, bool isPlaying)
    {
        OnEnvironmentSound?.Invoke(clip, mixerGroup, position,  isPlaying);
    }

    public static void EnvironmentParticles(ParticleSystem particles, bool isOn)
    {
        OnEnvironmentParticles?.Invoke(particles, isOn);
    }

    public static void PuzzleSuccess(AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position)
    {
        OnPuzzleSuccess?.Invoke(clip, mixerGroup, position);
    }
}
