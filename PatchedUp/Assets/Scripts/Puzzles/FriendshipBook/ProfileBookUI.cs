using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileBookUI : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] private ProfilePuzzleController controller;
    
    [Header("UI References")]
    [SerializeField] private GameObject canvasRoot;
    [SerializeField] private TMP_Text[] fieldLabels;
    [SerializeField] private TMP_InputField[] inputFields;
    [SerializeField] private Image[] fieldBackgrounds;
    [SerializeField] private Image profileImage;
    [SerializeField] private Button submitButton;
    
    [Header("Feedback Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color correctColor = new Color(0.6f, 1f, 0.6f, 0.8f);
    [SerializeField] private Color incorrectColor = new Color(1f, 0.6f, 0.6f, 0.8f);
    
    private bool initialized;
    
    private void Awake()
    {
        canvasRoot.SetActive(false);
        submitButton.onClick.AddListener(HandleSubmit);
    }
    
    private void OnDestroy()
    {
        if (submitButton != null) submitButton.onClick.RemoveListener(HandleSubmit);
    }
    
    public void Show()
    {
        if (!initialized) InitializeFromConfig();
        
        RestoreSavedState();
        canvasRoot.SetActive(true);
        
        // Focus the first non-validated field for keyboard flow
        FocusFirstUnvalidatedField();
    }
    
    public void Hide()
    {
        SaveCurrentInputsToController();
        canvasRoot.SetActive(false);
    }
    
    private void InitializeFromConfig()
    {
        var config = controller.Config;
        if (config == null) { return; }
        
        if (profileImage != null && config.profileImage != null)
            profileImage.sprite = config.profileImage;
        
        int count = Mathf.Min(config.questions.Length, fieldLabels.Length, inputFields.Length);
        
        for (int i = 0; i < count; i++)
        {
            fieldLabels[i].text = config.questions[i].fieldLabel + ":";
        }
        
        // Hide unused slots
        for (int i = count; i < fieldLabels.Length; i++)
        {
            fieldLabels[i].gameObject.SetActive(false);
            inputFields[i].gameObject.SetActive(false);
            if (i < fieldBackgrounds.Length && fieldBackgrounds[i] != null)
                fieldBackgrounds[i].gameObject.SetActive(false);
        }
        
        initialized = true;
    }
    
    private void RestoreSavedState()
    {
        int count = controller.Config.questions.Length;
        
        for (int i = 0; i < count; i++)
        {
            inputFields[i].text = controller.GetSavedAnswer(i);
            
            if (controller.IsFieldValidated(i))
            {
                SetFieldColor(i, correctColor);
                inputFields[i].interactable = false;
            }
            else
            {
                SetFieldColor(i, defaultColor);
                inputFields[i].interactable = true;
            }
        }
        
        submitButton.interactable = !controller.IsSolved;
    }
    
    private void SaveCurrentInputsToController()
    {
        if (!initialized) return;
        
        int count = controller.Config.questions.Length;
        for (int i = 0; i < count; i++)
        {
            controller.SaveAnswer(i, inputFields[i].text);
        }
    }
    
    private void HandleSubmit()
    {
        int count = controller.Config.questions.Length;
        var answers = new string[count];
        for (int i = 0; i < count; i++)
            answers[i] = inputFields[i].text;
        
        var result = controller.Submit(answers);
        
        
        for (int i = 0; i < count; i++)
        {
            if (result.validatedFields[i])
            {
                SetFieldColor(i, correctColor);
                inputFields[i].interactable = false;
            }
            else
            {
                SetFieldColor(i, incorrectColor);
            }
        }
        
        if (result.allCorrect)
        {
            submitButton.interactable = false;
        }
    }
    
    private void SetFieldColor(int index, Color color)
    {
        if (index < fieldBackgrounds.Length && fieldBackgrounds[index] != null)
            fieldBackgrounds[index].color = color;
    }
    
    private void FocusFirstUnvalidatedField()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].gameObject.activeInHierarchy && inputFields[i].interactable)
            {
                inputFields[i].Select();
                inputFields[i].ActivateInputField();
                return;
            }
        }
    }
}