using UnityEngine;
using StarterAssets;

public class InspectableItem : MonoBehaviour, IInteractable {
    [SerializeField] private GameObject infoOverlay;
    private StarterAssetsInputs _input;
    private bool _isOpen = false;

    private void Start() {
        _input = FindObjectOfType<StarterAssetsInputs>();
        if (infoOverlay != null) infoOverlay.SetActive(false);
    }

    public string GetHintText() => "Untersuchen";

    public void Interact() {
        _isOpen = !_isOpen;
        if (infoOverlay != null) infoOverlay.SetActive(_isOpen);

        // Cursor Management
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isOpen;

        // Spielerbewegung sperren
        _input.cursorInputForLook = !_isOpen;
    }
}