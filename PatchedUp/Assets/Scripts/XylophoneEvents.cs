using System;
using UnityEngine;
using UnityEngine.Audio;

public static class XylophoneEvents
{
    public static Action<AudioClip, AudioMixerGroup> OnPlayXylophoneKey;

    public static void PlayXylophoneKey(AudioClip audioClip, AudioMixerGroup audioMixerGroup)
    {
        OnPlayXylophoneKey?.Invoke(audioClip, audioMixerGroup);
    }
}
