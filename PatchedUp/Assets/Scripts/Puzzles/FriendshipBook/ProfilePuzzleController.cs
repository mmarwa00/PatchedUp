using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfilePuzzleController : MonoBehaviour
{
    [SerializeField] private ProfilePuzzleConfig config;
    [SerializeField] private GameObject rewardSticker;
    [SerializeField] private AudioSource audioSource;
    
    public event Action OnPuzzleSolved;
    
    private string[] savedAnswers;
    private bool[] fieldValidated;
    private bool isSolved;
    
    public ProfilePuzzleConfig Config => config;
    public bool IsSolved => isSolved;
    
    private void Awake()
    {
        if (config == null) return;
        
        savedAnswers = new string[config.questions.Length];
        fieldValidated = new bool[config.questions.Length];
        for (int i = 0; i < savedAnswers.Length; i++) savedAnswers[i] = "";
        
        if (rewardSticker != null) rewardSticker.SetActive(false);
    }
    
    public string GetSavedAnswer(int index) => savedAnswers[index];
    public bool IsFieldValidated(int index) => fieldValidated[index];
    
    public void SaveAnswer(int index, string answer)
    {
        savedAnswers[index] = answer;
    }
    
    public SubmitResult Submit(string[] currentAnswers)
    {
        if (isSolved) return new SubmitResult { wasAlreadySolved = true };
        
        for (int i = 0; i < config.questions.Length; i++)
        {
            savedAnswers[i] = currentAnswers[i];
            
            if (IsAnswerAcceptable(currentAnswers[i], config.questions[i]))
                fieldValidated[i] = true;
        }
        
        bool allCorrect = true;
        foreach (bool valid in fieldValidated)
            if (!valid) { allCorrect = false; break; }
        
        if (allCorrect)
        {
            isSolved = true;
            TriggerReward();
            OnPuzzleSolved?.Invoke();
        }
        
        return new SubmitResult
        {
            validatedFields = (bool[])fieldValidated.Clone(),
            allCorrect = allCorrect
        };
    }
    
    private bool IsAnswerAcceptable(string input, ProfilePuzzleConfig.ProfileQuestion q)
    {
        if (string.IsNullOrEmpty(input)) return false;
        
        string normalized = q.trimWhitespace ? input.Trim() : input;
        
        foreach (var accepted in q.acceptedAnswers)
        {
            string normalizedAccepted = q.trimWhitespace ? accepted.Trim() : accepted;
            
            bool match = q.caseSensitive
                ? normalized == normalizedAccepted
                : string.Equals(normalized, normalizedAccepted, 
                    System.StringComparison.OrdinalIgnoreCase);
            
            if (match) return true;
        }
        return false;
    }
    
    private void TriggerReward()
    {
        if (rewardSticker != null) rewardSticker.SetActive(true);
        if (audioSource != null && config.rewardSound != null)
            audioSource.PlayOneShot(config.rewardSound);
    }
    
    public struct SubmitResult
    {
        public bool[] validatedFields;
        public bool allCorrect;
        public bool wasAlreadySolved;
    }
}