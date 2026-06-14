using UnityEngine;

public class EnvPuzzleDestination : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzleManager.Instance.RegisterCompletedPuzzle(true);
            Destroy(gameObject);
        }
    }
}
