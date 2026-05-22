using UnityEngine;

[CreateAssetMenu(fileName = "ProfilePuzzle", menuName = "Puzzles/Profile Puzzle")]
public class ProfilePuzzleConfig : ScriptableObject
{
    [System.Serializable]
    public class ProfileQuestion
    {
        public string fieldLabel;
        public string[] acceptedAnswers;
        public bool caseSensitive = false;
        public bool trimWhitespace = true;
    }

    public string profileName;
    public Sprite profileImage;
    public ProfileQuestion[] questions;
    
    [Header("Reward")]
    public Sprite rewardSticker;
    public AudioClip rewardSound;
}
