using UnityEngine;

[CreateAssetMenu(fileName = "ToyBlocks", menuName = "Puzzles/ToyBlocks")]
public class ToyBlocks : ScriptableObject
{
    [System.Serializable]
    public struct Block
    {
        public string name;
    }
    
    [SerializeField] private Block[] blocks;
    
    public int Count => blocks.Length;
    public string GetName(int index) => blocks[index].name;
}
