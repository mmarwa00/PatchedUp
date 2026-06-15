using UnityEngine;

public class AbilityItem : MonoBehaviour, IPickable {
    [SerializeField] private string abilityName = "";
    private Rigidbody _rb;

    [Header("Journal Visuals")]
    [SerializeField] private Sprite itemIcon;

    public Sprite ItemIcon => itemIcon;
    public string AbilityName => abilityName;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    public string GetHintText() => $"[E] Take {abilityName}";

    public void Interact() {
        Debug.Log($"[AbilityItem] Interact ausgeführt für: {abilityName}");

        PlayerAbilityManager abilityManager = Object.FindAnyObjectByType<PlayerAbilityManager>();
        if (abilityManager != null) {
            abilityManager.OnItemPickedUp();
            Debug.Log($"[AbilityItem] {abilityName}-Visuals am Bären aktiviert!");
        }
    }

    public virtual void UseAbility(Camera mainCamera) {
        Debug.Log($"Used basic item: {abilityName}");
    }

    public void OnPickup() {
        if (_rb != null) {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        foreach (var col in GetComponentsInChildren<Collider>()) {
            col.enabled = false;
        }
    }

    public void OnDrop() {
        if (_rb != null) {
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }

        foreach (var col in GetComponentsInChildren<Collider>()) {
            col.enabled = true;
        }
    }
}