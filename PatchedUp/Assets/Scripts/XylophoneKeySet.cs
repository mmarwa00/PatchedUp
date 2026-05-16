using UnityEngine;

[CreateAssetMenu(fileName = "XylophoneKeySet", menuName = "Puzzles/XylophoneKeySet")]
public class XylophoneKeySet : ScriptableObject
{
    [System.Serializable]
    public struct Note
    {
        public string noteName;
        public AudioClip clip;
        public Color color;
    }

    [SerializeField] private Note[] notes;

    public int Count => notes.Length;
    public AudioClip GetClip(int index) => notes[index].clip;
    public Color GetColor(int index) => notes[index].color;
    public string GetName(int index) => notes[index].noteName;
}