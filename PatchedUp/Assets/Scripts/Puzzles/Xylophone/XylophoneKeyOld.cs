using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class XylophoneKeyOld : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    
    private AudioSource audioSource;
    private InputAction inputAction;

    private bool inRange;

    void Start()
    {
        this.inputAction = InputSystem.actions.FindAction("Interact");
    }
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }
    private void Update()
    {
            if (inRange && this.inputAction.WasPressedThisFrame())
            {
                PlayXylophoneKey();
            }
    }
    private void PlayXylophoneKey()
    {
        audioSource.PlayOneShot(audioClip);
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
