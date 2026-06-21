using System;
using UnityEngine;

public class EnvPuzzleDestination : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PuzzleManager.Instance != null)
            {
                PuzzleManager.Instance.RegisterCompletedPuzzle(true);
            }
            else
            {
                Debug.LogWarning("ManagerInstance==null");
            }
            
            PuzzleManager.Instance.RegisterReachedGameEnd(true);

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 90f);
    }
}
