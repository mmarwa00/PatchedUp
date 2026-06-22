using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : ManagerBase {
    private static PuzzleManager instance;
    public static PuzzleManager Instance => instance;

    [SerializeField] private int requiredPuzzleCount = 5;
    private int completedPuzzleCount = 0;
    private bool allPuzzlesCompletedFired = false;

    private HashSet<string> _solvedPuzzleIds = new HashSet<string>();

    public event Action<bool> OnPuzzleSolved;
    public event Action<bool> OnReachedGameEnd;
    public event Action OnAllPuzzlesCompleted;

    public int RequiredPuzzleCount => requiredPuzzleCount;
    public bool AllPuzzlesCompleted => completedPuzzleCount >= requiredPuzzleCount;

    public override IEnumerator Init() {
        instance = this;
        OnPuzzleSolved += HandlePuzzleSolved;
        OnReachedGameEnd += HandleGameEnd;
        Debug.Log("PuzzleManager initialized");
        yield break;
    }

    public override IEnumerator Load() {
        SaveData loadedData = SaveSystem.Load();
        if (loadedData != null) {
            _solvedPuzzleIds.Clear();
            if (loadedData.solvedPuzzleIds != null) {
                foreach (string id in loadedData.solvedPuzzleIds) {
                    _solvedPuzzleIds.Add(id);
                }
            }

            completedPuzzleCount = _solvedPuzzleIds.Count;
            Debug.Log($"[PuzzleManager] Restored {completedPuzzleCount} solved puzzles from save.");
            EvaluateAllPuzzlesCompleted();
        }
        yield break;
    }

    public bool IsPuzzleSolved(string puzzleId) {
        return _solvedPuzzleIds.Contains(puzzleId);
    }

    public List<string> GetSolvedPuzzleIds() {
        return new List<string>(_solvedPuzzleIds);
    }

    private void HandleGameEnd(bool gameEnd) {
        var ui = App.Instance.GetManager<UIManager>();
        ui.ShowYouWonScreen(true);
    }

    private void HandlePuzzleSolved(bool isSolved) {
        if (!isSolved) return;
        completedPuzzleCount++;
        Debug.Log("Puzzle-Count" + completedPuzzleCount);
        EvaluateAllPuzzlesCompleted();
    }

    private void EvaluateAllPuzzlesCompleted() {
        if (allPuzzlesCompletedFired) return;
        if (!AllPuzzlesCompleted) return;
        allPuzzlesCompletedFired = true;
        Debug.Log($"[PuzzleManager] All {requiredPuzzleCount} puzzles completed - unlocking door.");
        OnAllPuzzlesCompleted?.Invoke();
    }

    public void RegisterCompletedPuzzle(bool isCompleted) {
        this.OnPuzzleSolved?.Invoke(isCompleted);
    }

    public void RegisterCompletedPuzzle(string puzzleId) {
        if (_solvedPuzzleIds.Contains(puzzleId)) {
            Debug.Log($"[PuzzleManager] Puzzle '{puzzleId}' already solved, ignoring.");
            return;
        }
        _solvedPuzzleIds.Add(puzzleId);
        this.OnPuzzleSolved?.Invoke(true);
    }

    public void RegisterReachedGameEnd(bool isReachedGameEnd) {
        this.OnReachedGameEnd?.Invoke(isReachedGameEnd);
    }

    public int GetCompletedPuzzleCount() {
        return completedPuzzleCount;
    }
}