using UnityEngine;
using StarterAssets;

public class CaptureSystem : MonoBehaviour
{
    private int _catchCount = 0;
    private FirstPersonController _controller;
    private CharacterController _characterController;
    private Animator _animator; // <-- ADD THIS LINE
    private bool _alreadyCaught = false;

    [SerializeField] private Transform respawnPoint;

    private void Start()
    {
        _controller = GetComponent<FirstPersonController>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>(); // <-- ADD THIS LINE
    }

    public void OnCaught()
    {
        if (_alreadyCaught) return;
        _alreadyCaught = true;
        _catchCount++;

        if (_catchCount == 1)
        {
            Debug.Log("Erwischt! Gliedmaßenverlust — langsamer.");
            _controller.MoveSpeedValue *= 0.5f;
            _controller.SprintSpeedValue *= 0.5f;
            Respawn();
            _alreadyCaught = false;
        }
        else if (_catchCount >= 2)
        {
            Debug.Log("Game Over!");

           
            if (_animator != null)
            {
                _animator.SetTrigger("Die");
            }
            // Disable movement so the player can't walk away while dead
            _characterController.enabled = false;

            // Your scene management line can un-comment here when you create the scene:
            // UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    private void Respawn()
    {
        _characterController.enabled = false;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        _characterController.enabled = true;

        Debug.Log("Respawned!");
    }
}