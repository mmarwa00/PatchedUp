using System;
using UnityEngine;
using UnityEngine.Audio;

public class XylophoneSoundObserver:  MonoBehaviour
{

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        XylophoneEvents.OnPlayXylophoneKey += HandleXylophoneSound;
        XylophoneEvents.OnPuzzleSuccess += PlayPuzzleSuccessSound;
    }

    private void OnDisable()
    {
        XylophoneEvents.OnPlayXylophoneKey -= HandleXylophoneSound;
        XylophoneEvents.OnPuzzleSuccess -= PlayPuzzleSuccessSound;
    }

    private void HandleXylophoneSound(AudioClip clip, AudioMixerGroup audioMixerGroup)
    {
        if (clip == null) return;
        
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.PlayOneShot(clip);
        
    }

    private void PlayPuzzleSuccessSound(AudioClip clip, AudioMixerGroup audioMixerGroup)
    {
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.PlayOneShot(clip);
    }
}
