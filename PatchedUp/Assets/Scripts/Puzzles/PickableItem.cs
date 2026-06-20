using UnityEngine;

public class PickableItem : MonoBehaviour, IPickable {
    private Rigidbody _rb;

    // Pickability is independent of the collider, so an item can stay solid (climbable / blocks the
    // camera) while being locked against pickup. The tower puzzle uses this to keep a finished stack solid.
    public bool IsPickable { get; private set; } = true;
    public void SetPickable(bool value) => IsPickable = value;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    public string GetHintText() => "Aufheben";

    // wird von PlayerInteraction �berschrieben
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