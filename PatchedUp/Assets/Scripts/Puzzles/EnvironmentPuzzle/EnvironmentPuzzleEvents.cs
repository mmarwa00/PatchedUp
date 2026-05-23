using System;
using UnityEngine;
using UnityEngine.Audio;

public static class EnvironmentPuzzleEvents
{
    public static Action<Light, bool, float, float> OnToggleLights;

    public static Action<AudioClip, AudioMixerGroup, Vector3> OnEnvironmentSound;
    
    public static Action<ParticleSystem, bool> OnEnvironmentParticles;

    public static void ToggleLights(Light light, bool on, float minIntensity, float maxIntensity)
    {
        OnToggleLights?.Invoke(light, on,  minIntensity, maxIntensity);
    }

    public static void EnvironmentSound(AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position)
    {
        OnEnvironmentSound?.Invoke(clip, mixerGroup, position);
    }

    public static void EnvironmentParticles(ParticleSystem particles, bool isOn)
    {
        OnEnvironmentParticles?.Invoke(particles, isOn);
    }
}
