using System;
using UnityEngine;
using UnityEngine.Audio;

public static class XylophoneEvents
{
    public static Action<AudioClip, AudioMixerGroup> OnPlayXylophoneKey;
    
    public static Action<AudioClip, AudioMixerGroup> OnPuzzleSuccess;

    public static Action<int> OnPlayerNoteInput;

    public static Action<int> OnVisualEffect;

    public static void PlayXylophoneKey(AudioClip audioClip, AudioMixerGroup audioMixerGroup)
    {
        OnPlayXylophoneKey?.Invoke(audioClip, audioMixerGroup);
    }

    public static void PlayerNoteInput(int keyIndex)
    {
        OnPlayerNoteInput?.Invoke(keyIndex);
    }

    public static void VisualEffect(int keyIndex)
    {
        OnVisualEffect?.Invoke(keyIndex);
    }

    public static void PuzzleSuccess(AudioClip clip, AudioMixerGroup audioMixerGroup)
    {
        OnPuzzleSuccess?.Invoke(clip, audioMixerGroup);
    }
}
