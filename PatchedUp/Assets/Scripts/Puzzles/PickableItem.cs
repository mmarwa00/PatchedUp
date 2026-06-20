using UnityEngine;

public class PickableItem : MonoBehaviour, IPickable {
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    public string GetHintText() => "Aufheben";

    // wird von PlayerInteraction überschrieben
    public void Interact() { }

    public void OnPickup() {
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    public void OnDrop() {
        _rb.isKinematic = false;
        _rb.useGravity = true;
    }
}