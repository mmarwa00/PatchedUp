using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TeaCupPuzzleController : MonoBehaviour
{
    [SerializeField] private GameObject[] cups;
    private Dictionary<int, bool> cupArrangement;
    private bool solved;
    
    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    private AudioSource audioSource;


    private void Start() {
        audioSource = GetComponent<AudioSource>();
        cupArrangement = new Dictionary<int, bool>();
        int cupAmount = cups.Length;
        for (int i = 0; i < cupAmount; i++) {
            cupArrangement.Add(i, false);
        }
    }

    public bool CheckSolved() {
        if (!solved && PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleSolved(gameObject.name)) {
            solved = true;
            LockCups();
            Debug.Log($"[Load] {gameObject.name} war bereits gelöst!");
        }
        return solved;
    }
    private void OnEnable()
    {
        TeaCupPuzzleEvents.OnSnap += HandleSnapEvent;
    }

    private void OnDisable()
    {
        TeaCupPuzzleEvents.OnSnap -= HandleSnapEvent;
    }

    private void HandleSnapEvent(bool snapState,  int cupId)
    {
        if (CheckSolved()) return;
        cupArrangement[cupId] = snapState;
        CheckArrangement();
    }

    private void CheckArrangement()
    {
        int placedCups = 0;
        for (int i = 0; i < cups.Length; i++)
        {
            if (cupArrangement[i])
            {
                placedCups++;
            }
        }

        if (placedCups == cups.Length)
        {
            OnFinishedArrangement();
        }
    }

    private void OnFinishedArrangement()
    {
        if (solved) return;
        solved = true;

        audioSource.PlayOneShot(audioClip);
        LockCups();
        PuzzleManager.Instance.RegisterCompletedPuzzle(gameObject.name);
        GameManager gm = App.Instance.GetManager<GameManager>();
        if (gm != null) gm.SaveGame();
    }
    
    private void LockCups()
    {
        foreach (GameObject cup in cups)
        {
            if (cup == null) continue;
            PickableItem pickable = cup.GetComponent<PickableItem>();
            if (pickable != null) pickable.SetPickable(false);
        }
    }
}
