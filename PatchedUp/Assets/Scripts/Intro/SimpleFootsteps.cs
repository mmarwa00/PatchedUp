using UnityEngine;

public class SimpleFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    public float stepRate = 0.5f; // Lower number = faster footsteps

    private Vector3 _lastPosition;
    private float _nextStepTime;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        // Check if the child actually moved since the last frame
        if (Vector3.Distance(transform.position, _lastPosition) > 0.01f)
        {
            // Check if enough time has passed to play the next step sound
            if (Time.time >= _nextStepTime)
            {
                footstepSource.Play();
                _nextStepTime = Time.time + stepRate;
            }
        }

        // Update the position tracker
        _lastPosition = transform.position;
    }
}