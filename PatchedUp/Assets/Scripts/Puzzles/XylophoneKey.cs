using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class XylophoneKey : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private int keyIndex;
    [SerializeField] private XylophoneGlowEffect visualEffect;
    
    private AudioSource audioSource;
    private InputAction inputAction;

    private bool inRange;

    void Start()
    {
        this.inputAction = InputSystem.actions.FindAction("Interact");
    }
    private void Update()
    {
        if (inRange && this.inputAction.WasPressedThisFrame())
        {
            XylophoneEvents.VisualEffect(keyIndex);
            XylophoneEvents.PlayXylophoneKey(audioClip, audioMixerGroup);
            XylophoneEvents.PlayerNoteInput(keyIndex);
        }
    }

    private void OnEnable()
    {
        XylophoneEvents.OnVisualEffect += HandleVisualEffect;
    }

    private void OnDisable()
    {
        XylophoneEvents.OnVisualEffect -= HandleVisualEffect;
    }

    private void HandleVisualEffect(int index)
    {
        if(index == keyIndex) visualEffect.PlayGlow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
