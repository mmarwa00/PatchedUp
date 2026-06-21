using System;
using System.Collections;
using UnityEngine;

public class PuzzleManager : ManagerBase
{
    private static PuzzleManager instance;
    public static PuzzleManager Instance => instance;
    private int completedPuzzleCount = 0;
    
    public event Action<bool> OnPuzzleSolved;
    public event Action<bool> OnReachedGameEnd;
    
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
