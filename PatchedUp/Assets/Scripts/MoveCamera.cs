using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform headPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = headPosition.position;
    }
}
