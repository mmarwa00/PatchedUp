using System;
using System.Collections;
using UnityEngine;

public class PuzzleManager : ManagerBase
{
    private static PuzzleManager instance;
    public static PuzzleManager Instance => instance;
    private int completedPuzzleCount = 0;
    
    public event Action<bool> OnPuzzleSolved;
    
    public override IEnumerator Init()
    {
        instance = this;
        OnPuzzleSolved += HandlePuzzleSolved;
        Debug.Log("PuzzleManager initialized");
        yield break;
    }

    public override IEnumerator Load()
    {
        yield break;
    }

    private void HandlePuzzleSolved(bool isSolved)
    {
        if (!isSolved) return;
        
        completedPuzzleCount++;
    }
    public void RegisterCompletedPuzzle(bool isCompleted)
    {
        this.OnPuzzleSolved?.Invoke(isCompleted);
    }

    public int GetCompletedPuzzleCount()
    {
        return completedPuzzleCount;
    }
    
}
