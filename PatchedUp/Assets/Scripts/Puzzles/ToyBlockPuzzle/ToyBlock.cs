using UnityEngine;

public class ToyBlock : MonoBehaviour
{
    [SerializeField] private char blockLetter;
    
    public char GetBlockLetter() => blockLetter;
}
