using System;
using UnityEngine;
using UnityEngine.Audio;

public static class EnvironmentPuzzleEvents
{
    public static Action<Light, bool> OnToggleLights;

    public static Action<AudioClip, AudioMixerGroup, Vector3> OnEnvironmentSound;

    public static void ToggleLights(Light light, bool on)
    {
        OnToggleLights?.Invoke(light, on);
    }

    public static void EnvironmentSound(AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position)
    {
        OnEnvironmentSound?.Invoke(clip, mixerGroup, position);
    }
}
