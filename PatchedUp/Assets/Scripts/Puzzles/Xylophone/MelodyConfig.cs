using UnityEngine;

[CreateAssetMenu(fileName = "MelodyConfig", menuName = "Puzzles/Melody")]
public class MelodyConfig : ScriptableObject
{
    [SerializeField] private string melodyName;
    [SerializeField] private int[] noteSequence;
    [SerializeField] private float playbackSpeed = 1f;
    [SerializeField] private bool randomize;
    [SerializeField] private int randomLength = 5;
    [SerializeField] private bool avoidRepeat = true;
    
    private static readonly int[] PentatonicIndices = { 0, 1, 2, 4, 5, 7 };
    
    public int[] GetSequence()
    {
        if (randomize)
        {
            var seq = new int[randomLength];
            for (int i = 0; i < randomLength; i++)
            {
                seq[i] = Random.Range(0, 7);
                return seq;
            }
        }
        return (int[])noteSequence.Clone();
    }
    
    public int[] GetRandomSequence(int customRandomLength)
    {
        if (randomize)
        {
            var seq = new int[customRandomLength];
            for (int i = 0; i < customRandomLength; i++)
            {
                seq[i] = Random.Range(0, 7);
            }
            return seq;
            
        }
        return (int[])noteSequence.Clone();
    }
    
    private int[] GeneratePentatonic(int length)
    {
        var sequence = new int[length];
        int previous = -1;

        for (int i = 0; i < length; i++)
        {
            int candidate;
            int safety = 0;
            do
            {
                candidate = PentatonicIndices[Random.Range(0, PentatonicIndices.Length)];
                safety++;
            }
            while (avoidRepeat && candidate == previous && safety < 10);

            sequence[i] = candidate;
            previous = candidate;
        }

        return sequence;
    }
    
    public float GetPlaybackSpeed()
    {
        return playbackSpeed;
    }
    
    public int GetSequenceLength()
    {
        return noteSequence.Length;
    }
    
    public int[] GetPentatonicSequence(int length)
    {
        return GeneratePentatonic(length);
    }
}