using UnityEngine;
using StarterAssets;

public class CaptureSystem : MonoBehaviour {
    private int _catchCount = 0;
    private FirstPersonController _controller;
    private CharacterController _characterController;
    private bool _alreadyCaught = false;

    [SerializeField] private Transform respawnPoint;

    private void Start() {
        _controller = GetComponent<FirstPersonController>();
        _characterController = GetComponent<CharacterController>();
    }

    public void OnCaught() {
        if (_alreadyCaught) return;
        _alreadyCaught = true;
        _catchCount++;

        if (_catchCount == 1) {
            Debug.Log("Erwischt! Gliedmaßenverlust — langsamer.");
            _controller.MoveSpeedValue *= 0.5f;
            _controller.SprintSpeedValue *= 0.5f;
            Respawn();
            _alreadyCaught = false;
        }
        else if (_catchCount >= 2) {
            Debug.Log("Game Over!");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    private void Respawn() {
        _characterController.enabled = false;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        _characterController.enabled = true;

        Debug.Log("Respawned!");
    }
}