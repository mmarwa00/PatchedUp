using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileBookUIFiller : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ProfilePuzzleConfig config;
    
    [Header("Label References")]
    [SerializeField] private TMP_Text[] fieldLabels;
    
    [Header("Input Field References")]
    [SerializeField] private TMP_InputField[] inputFields;
    
    [Header("Profile Display")]
    [SerializeField] private Image profileImage;
    
    [Header("Submit")]
    [SerializeField] private Button submitButton;
    
    void Start()
    {
        ApplyConfig();
    }
    
    private void ApplyConfig()
    {
        if (config == null)
        {
            Debug.LogError("ProfileBookUI: No config assigned!");
            return;
        }
        
        // Set profile header
        if (profileImage != null && config.profileImage != null) 
            profileImage.sprite = config.profileImage;
        
        // Populate field labels and clear input fields
        int count = Mathf.Min(config.questions.Length, fieldLabels.Length, inputFields.Length);
        
        for (int i = 0; i < count; i++)
        {
            fieldLabels[i].text = config.questions[i].fieldLabel + ":";
            inputFields[i].text = "";
        }
        
        // Disable any unused fields (if config has fewer questions than UI slots)
        for (int i = count; i < fieldLabels.Length; i++)
        {
            fieldLabels[i].gameObject.SetActive(false);
            inputFields[i].gameObject.SetActive(false);
        }
    }
}