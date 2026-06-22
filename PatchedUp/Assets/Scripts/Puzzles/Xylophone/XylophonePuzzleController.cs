using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class XylophonePuzzleController : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [SerializeField] private MelodyConfig melodyConfig;
    [SerializeField] private XylophoneKeySet keySet;
    [SerializeField] private AudioMixerGroup audioMixer;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;

    [Header("Player Input")]
    private int[] targetSequence;
    private readonly List<int> playerInput = new  List<int>();
    
    private enum State {Idle, Playing, Listening, Solved, Failed}
    private State state;
    private int melodyIterationAmount;
    private int successIterationAmount;
    private InputAction inputAction;
    private InputAction interactAction;
    private bool puzzleIsRunning;

    void Start() {
        this.interactAction = InputSystem.actions.FindAction("Interact");
        targetSequence = melodyConfig.GetSequence();
        successIterationAmount = 0;
        puzzleIsRunning = false;
    }

    private bool CheckSolved() {
        if (!puzzleIsRunning && PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleSolved(gameObject.name)) {
            puzzleIsRunning = true;
            Debug.Log($"[Load] {gameObject.name} war bereits gelöst!");
        }
        return puzzleIsRunning;
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && this.interactAction.WasPressedThisFrame()) {
            if (CheckSolved()) return;
            StartCoroutine(WaitThenStart());
            puzzleIsRunning = true;
        }
    }

    void OnEnable()
    {
        XylophoneEvents.OnPlayerNoteInput += HandlePlayerInput;
    }

    void OnDisable()
    {
        XylophoneEvents.OnPlayerNoteInput -= HandlePlayerInput;
    }

    private void HandlePlayerInput(int keyIndex)
    {
        if (state != State.Playing) return;
        
        playerInput.Add(keyIndex);
        
        int position = playerInput.Count - 1;
        if (playerInput[position] != targetSequence[position])
        {
            OnFailed();
            return;
        }

        if (playerInput.Count == targetSequence.Length)
        {
            OnSolved();
        }
    }

    private IEnumerator PlayPuzzleMelody()
    {
        playerInput.Clear();
        state = State.Listening;

        switch (successIterationAmount)
        {
            case 0: targetSequence = melodyConfig.GetSequence(); break;
            case 1: targetSequence = melodyConfig.GetPentatonicSequence(4); break;
            case 2: targetSequence = melodyConfig.GetPentatonicSequence(5); break;
            default: targetSequence = melodyConfig.GetSequence(); break;
        }
        
        foreach (int keyIndex in targetSequence)
        {
            var clip = keySet.GetClip(keyIndex);
            XylophoneEvents.VisualEffect(keyIndex);
            XylophoneEvents.PlayXylophoneKey(clip, audioMixer);
            yield return new WaitForSeconds(melodyConfig.GetPlaybackSpeed());
        }
        
        state = State.Playing;
    }

    private void OnFailed()
    {
        state = State.Failed;
        //successIterationAmount = 0;
        playerInput.Clear();
        StartCoroutine(RestartOnFailure());
    }

    private IEnumerator RestartOnFailure()
    {
        yield return new WaitForSeconds(1f);
        XylophoneEvents.PuzzleSuccess(failureSound, audioMixer);
        yield return new WaitForSeconds(3f);
        yield return PlayPuzzleMelody();
    }
    
    private IEnumerator HandleSuccess()
    {
        yield return new WaitForSeconds(2f);
        XylophoneEvents.PuzzleSuccess(successSound, audioMixer);
        
        if (successIterationAmount == 3)
        {
            PuzzleComplete();
            yield break;
        }
        
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(PlayPuzzleMelody());
        
    }

    private void OnSolved()
    {
        state = State.Solved;
        successIterationAmount++;
        StartCoroutine(HandleSuccess());
    }
    
    private IEnumerator WaitThenStart()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(PlayPuzzleMelody());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            puzzleIsRunning = false;  
        }
        
    }

    private void PuzzleComplete()
    {
        Debug.Log("Puzzle Complete");
        PuzzleManager.Instance.RegisterCompletedPuzzle(gameObject.name);
        GameManager gm = App.Instance.GetManager<GameManager>();
        if (gm != null) gm.SaveGame();
    }
}
