using UnityEngine;
using System.Collections;
using StarterAssets;

public class CaptureSystem : MonoBehaviour
{
    private int _catchCount = 0;
    private FirstPersonController _controller;
    private CharacterController _characterController;
    private Animator _animator;
    private bool _alreadyCaught = false;

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float caughtScreenDuration = 3.5f;

    private void Start()
    {
        _controller = GetComponent<FirstPersonController>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

    }

    public void OnCaught() {
        if (_alreadyCaught) return;
        _alreadyCaught = true;

        GameManager gm = App.Instance.GetManager<GameManager>();
        if (gm != null) {
            gm.PlayerCaught(this);
        }
    }

    public void ApplySpeedPenalty(float factor) {
        if (_controller != null) {
            _controller.MoveSpeedValue *= factor;
            _controller.SprintSpeedValue *= factor;
        }
    }

    public void StartRespawnSequence() {
        StartCoroutine(ShowCaughtScreenAndRespawn());
    }

    public void FreezePlayer() {
        if (_animator != null) _animator.SetTrigger("Die");
        if (_characterController != null) _characterController.enabled = false;
        if (_controller != null) _controller.enabled = false;
    }

    private IEnumerator ShowCaughtScreenAndRespawn() {
        UIManager ui = App.Instance.GetManager<UIManager>();
        if (ui != null) ui.ShowCaughtScreen(true);

        if (_controller != null) _controller.enabled = false;

        yield return new WaitForSeconds(caughtScreenDuration);

        if (_characterController != null) _characterController.enabled = false;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        if (_characterController != null) _characterController.enabled = true;

        if (ui != null) ui.ShowCaughtScreen(false);
        if (_controller != null) _controller.enabled = true;

        _alreadyCaught = false;
    }
    public void RestartGameButton() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitGameButton() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}