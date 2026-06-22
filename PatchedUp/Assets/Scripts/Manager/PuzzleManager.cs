using System;
using System.Collections;
using UnityEngine;

public class PuzzleManager : ManagerBase
{
    private static PuzzleManager instance;
    public static PuzzleManager Instance => instance;
    
    [SerializeField] private int requiredPuzzleCount = 5;

    private int completedPuzzleCount = 0;
    private bool allPuzzlesCompletedFired = false;

    public event Action<bool> OnPuzzleSolved;
    public event Action<bool> OnReachedGameEnd;
    
    public event Action OnAllPuzzlesCompleted;

    public int RequiredPuzzleCount => requiredPuzzleCount;
    public bool AllPuzzlesCompleted => completedPuzzleCount >= requiredPuzzleCount;
    
    public override IEnumerator Init()
    {
        instance = this;
        OnPuzzleSolved += HandlePuzzleSolved;
        OnReachedGameEnd += HandleGameEnd;
        Debug.Log("PuzzleManager initialized");
        yield break;
    }

    public override IEnumerator Load()
    {
        SaveData loadedData = SaveSystem.Load();
        if (loadedData != null)
        {
            completedPuzzleCount = loadedData.solvedPuzzlesCount;
            Debug.Log($"[PuzzleManager] Restored {completedPuzzleCount} solved puzzles from save.");
            EvaluateAllPuzzlesCompleted();
        }
        yield break;
    }

    private void HandleGameEnd(bool gameEnd)
    {
        var ui = App.Instance.GetManager<UIManager>();
        ui.ShowYouWonScreen(true);
    }

    private void HandlePuzzleSolved(bool isSolved)
    {
        if (!isSolved) return;

        completedPuzzleCount++;
        Debug.Log("Puzzle-Count" + completedPuzzleCount);
        EvaluateAllPuzzlesCompleted();
    }

    private void EvaluateAllPuzzlesCompleted()
    {
        if (allPuzzlesCompletedFired) return;
        if (!AllPuzzlesCompleted) return;

        allPuzzlesCompletedFired = true;
        Debug.Log($"[PuzzleManager] All {requiredPuzzleCount} puzzles completed - unlocking door.");
        OnAllPuzzlesCompleted?.Invoke();
    }
    public void RegisterCompletedPuzzle(bool isCompleted)
    {
        this.OnPuzzleSolved?.Invoke(isCompleted);
    }

    public void RegisterReachedGameEnd(bool isReachedGameEnd)
    {
        this.OnReachedGameEnd?.Invoke(isReachedGameEnd);
    }

    public int GetCompletedPuzzleCount()
    {
        return completedPuzzleCount;
    }
    
}
