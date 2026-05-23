using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class EnvironmentPuzzleController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] private CharacterController characterController;
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
    [SerializeField] private AudioClip puzzleSuccessClip;
    private Vector3 audioLocation;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem particles;
    private bool particleIsOn = false;
    
    [Header("Puzzle")]
    [SerializeField, Range(0f, 3f)] private float inputGracePeriod;
    [SerializeField] private Transform resetLocation;
    private bool reachedEnd = false;
    [SerializeField] private GameObject finalDestination;
    private bool checkPlayerInput;
    private bool eventPlaying;
    private float lastCueChangeTime;
    private bool puzzleRunning = false;
    private bool soundIsPlaying = false;

    private void Update()
    {
        
        
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
            CheckIfWon();
            yield return null;
            
            if(!eventPlaying) continue;
            if(Time.time < lastCueChangeTime + inputGracePeriod) continue;
            
            if (player.CurrentMovementState != expectedState)
            {
                characterController.enabled = false;
                playerCapsule.transform.position = resetLocation.position;
                characterController.enabled = true;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator RunPuzzle()
    {
        while (!reachedEnd)
        {
            // if (!eventPlaying)
            // {
            
            int randomEvent = Random.Range(0, 3);

            switch (randomEvent)
            {
                case 0: yield return LightsEvent(); break;
                case 1: yield return SoundEvent(); break;
                case 2: yield return ParticleEvent(); break;
            }
                
            yield return new WaitForSeconds(0.5f);
            // }
            
        }
    }

    private void StartPuzzle()
    {
        StartCoroutine(RunPuzzle());
        StartCoroutine(CheckPlayerInput());
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !puzzleRunning)
        {
            Debug.Log("Puzzle triggered");
            reachedEnd = false;
            puzzleRunning = true;
            StartPuzzle();
        }
    }

    private void ResetGracePeriodTimer()
    {
        lastCueChangeTime = Time.time;
    }

    private IEnumerator LightsEvent()
    {
        Debug.Log("Lights triggered");
        int duration = Random.Range(1, 4);
        
        eventPlaying = true;
        EnvironmentPuzzleEvents.ToggleLights(lamp, lightIsOn, minIntensity, maxIntensity);
        expectedState = FirstPersonController.PlayerMovementState.Walking;
        ResetGracePeriodTimer();
        lightIsOn = true;
        
        yield return new WaitForSeconds(duration);
        
        EnvironmentPuzzleEvents.ToggleLights(lamp, lightIsOn, minIntensity, maxIntensity);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        ResetGracePeriodTimer();
        lightIsOn = false;
        eventPlaying = false;
        
    }
    
    private IEnumerator SoundEvent()
    {
        Debug.Log("Sound triggered");
        int duration = Random.Range(1, 4);
        
        eventPlaying = true;
        audioLocation = playerCapsule.transform.position;
        EnvironmentPuzzleEvents.EnvironmentSound(audioClip, audioMixerGroup, audioLocation, soundIsPlaying);
        expectedState = FirstPersonController.PlayerMovementState.Sprinting;
        ResetGracePeriodTimer();
        soundIsPlaying = true;
        
        yield return new WaitForSeconds(duration);
        
        EnvironmentPuzzleEvents.EnvironmentSound(audioClip, audioMixerGroup, audioLocation, soundIsPlaying);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        ResetGracePeriodTimer();
        eventPlaying = false;
        soundIsPlaying = false;
        
    }

    private IEnumerator ParticleEvent()
    {
        Debug.Log("Particle triggered");
        int duration = Random.Range(1, 4);
        
        eventPlaying = true;
        EnvironmentPuzzleEvents.EnvironmentParticles(particles, particleIsOn);
        expectedState = FirstPersonController.PlayerMovementState.CrouchWalking;
        ResetGracePeriodTimer();
        particleIsOn = true;
        
        yield return new WaitForSeconds(duration);
        
        EnvironmentPuzzleEvents.EnvironmentParticles(particles, particleIsOn);
        expectedState = FirstPersonController.PlayerMovementState.Idle;
        ResetGracePeriodTimer();
        particleIsOn = false;
        eventPlaying = false;
    }

    private void CheckIfWon()
    {
        audioLocation = playerCapsule.transform.position;
        if (finalDestination == null)
        {
            reachedEnd = true;
            StopCoroutine(CheckPlayerInput());
            StopCoroutine(RunPuzzle());
            EnvironmentPuzzleEvents.PuzzleSuccess(puzzleSuccessClip, audioMixerGroup, audioLocation);
        }
        
    }
    
    
}
