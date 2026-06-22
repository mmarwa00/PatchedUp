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

    public string GetHintText() => $"[E] Collect {abilityName}";

    public void Interact() {
        Debug.Log($"[AbilityItem] Interact ausgeführt für: {abilityName}");

        PlayerAbilityManager abilityManager = Object.FindAnyObjectByType<PlayerAbilityManager>();
        Debug.Log($"[AbilityItem] AbilityManager gefunden: {abilityManager != null}");
        if (abilityManager == null) return;

        PlayerInventory inventory = abilityManager.GetComponent<PlayerInventory>();
        Debug.Log($"[AbilityItem] Inventory gefunden: {inventory != null}");
        Debug.Log($"[AbilityItem] 'this' Instanz: {this.gameObject.name}, bereits im Bag: {inventory?.ItemsInBag.Contains(this)}");

        if (inventory != null && !inventory.ItemsInBag.Contains(this)) {
            inventory.AddItem(this);
            Debug.Log($"[AbilityItem] Nach AddItem – Bag Count: {inventory.ItemsInBag.Count}");
        }

        OnPickup();
        abilityManager.OnItemPickedUp();
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