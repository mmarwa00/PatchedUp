using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class EnvironmentPuzzleController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] private GameObject playerCapsule;
    private FirstPersonController.PlayerMovementState expectedState;
    
    [Header("Light")]
    [SerializeField] private Light lamp;
    [SerializeField, Range(0f, 15f)] private float minIntensity;
    [SerializeField, Range(0f, 15f)] private float maxIntensity;
    private bool lightIsOn = false;
    
    [Header("Sounds")]
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private AudioClip audioClip;
    private Vector3 audioLocation;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem particles;
    private bool particleIsOn = false;
    
    [Header("Puzzle")]
    [SerializeField, Range(0f, 3f)] private float inputGracePeriod;
    [SerializeField] private Transform resetLocation;
    private bool reachedEnd = false;
    [SerializeField] private GameObject finalDestination;
    private int eventDuration;
    private bool checkPlayerInput;
    private bool eventPlaying;

    private void Update()
    {
        eventDuration = Random.Range(1, 3);
    }

    private void Start()
    {
        lamp.enabled = false;
        particles.Stop();
    }

    private IEnumerator CheckPlayerInput()
    {
        while (!reachedEnd)
        {
            if (eventPlaying == false)
            {
                yield return new WaitForSeconds(inputGracePeriod);
            }
            if (player.CurrentMovementState != expectedState)
            {
                playerCapsule.transform.position = resetLocation.position;
            }
        }
    }

    private void StartPuzzle()
    {
        StartCoroutine(CheckPlayerInput());
        while (!reachedEnd)
        {
            if (!eventPlaying)
            {
                int randomEvent = Random.Range(0, 2);

                switch (randomEvent)
                {
                    case 0: StartCoroutine(LightsEvent()); break;
                    case 1: StartCoroutine(SoundEvent()); break;
                    case 2: StartCoroutine(ParticleEvent()); break;
                }
                {
                    
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reachedEnd = false;
            StartPuzzle();
        }
    }

    private IEnumerator LightsEvent()
    {
        eventPlaying = true;
        EnvironmentPuzzleEvents.ToggleLights(lamp, lightIsOn, minIntensity, maxIntensity);
        expectedState = FirstPersonController.PlayerMovementState.Walking;
        lightIsOn = true;
        yield return new WaitForSeconds(eventDuration);
        EnvironmentPuzzleEvents.ToggleLights(lamp, lightIsOn, minIntensity, maxIntensity);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        lightIsOn = false;
        eventPlaying = false;
        
    }
    
    private IEnumerator SoundEvent()
    {
        eventPlaying = true;
        audioLocation = playerCapsule.transform.position;
        EnvironmentPuzzleEvents.EnvironmentSound(audioClip, audioMixerGroup, audioLocation);
        expectedState = FirstPersonController.PlayerMovementState.Sprinting;
        yield return new WaitForSeconds(eventDuration);
        EnvironmentPuzzleEvents.EnvironmentSound(audioClip, audioMixerGroup, audioLocation);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        eventPlaying = false;
        
    }

    private IEnumerator ParticleEvent()
    {
        eventPlaying = true;
        EnvironmentPuzzleEvents.EnvironmentParticles(particles, particleIsOn);
        expectedState = FirstPersonController.PlayerMovementState.CrouchWalking;
        particleIsOn = true;
        yield return new WaitForSeconds(eventDuration);
        EnvironmentPuzzleEvents.EnvironmentParticles(particles, particleIsOn);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        particleIsOn = false;
        eventPlaying = false;
    }
    
    
}
