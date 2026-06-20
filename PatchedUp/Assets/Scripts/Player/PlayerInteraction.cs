using UnityEngine;
using StarterAssets;

public class PlayerInteraction : MonoBehaviour {


    [SerializeField] private float interactionDistance = 5.0f;
    [SerializeField] private Transform handPosition;

    private Camera _mainCamera;
    private StarterAssetsInputs _input;
    private PlayerInventory _inventory;
    private int currentEquippedIndex = 0;
    private IPickable _carriedItem;

    private void Start() {
        _mainCamera = Camera.main;
        _input = GetComponent<StarterAssetsInputs>();
        _inventory = GetComponent<PlayerInventory>();

        Debug.Log($"[PlayerInteraction] Start: camera={_mainCamera != null}, input={_input != null}, inventory={_inventory != null}");
    }

    private void Update() {
        if (_input.interact) {
            _input.interact = false;

            if (_carriedItem != null) {
                Drop();
                return;
            }

            // Erst Raycast versuchen
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance)) {
                Debug.Log($"[PlayerInteraction] Raycast trifft: {hit.collider.gameObject.name}");
                if (TryInteractWith(hit.collider.gameObject)) return;
            }

            // Fallback: n�chstes Item per OverlapSphere
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance, ~0, QueryTriggerInteraction.Collide);
            GameObject closest = null;
            float closestDist = float.MaxValue;
            foreach (var col in hits) {
                if (col.gameObject == gameObject) continue;
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closest = col.gameObject;
                }
            }
            if (closest != null) TryInteractWith(closest);
        }

        if (_carriedItem != null) {
            Transform itemTransform = (_carriedItem as MonoBehaviour).transform;
            itemTransform.position = handPosition.position;
            itemTransform.rotation = handPosition.rotation;
        }
    }

    private bool TryInteractWith(GameObject obj) {
        AbilityItem abilityItem = obj.GetComponent<AbilityItem>();
        if (abilityItem == null) abilityItem = obj.GetComponentInParent<AbilityItem>();
        if (abilityItem != null) {
            PickupToInventory(abilityItem);
            return true;
        }

        IPickable pickable = obj.GetComponent<IPickable>();
        if (pickable == null) pickable = obj.GetComponentInParent<IPickable>();
        if (pickable != null && (!(pickable is PickableItem pickItem) || pickItem.IsPickable)) {
            Pickup(pickable, obj);
            return true;
        }

        IInteractable interactable = obj.GetComponent<IInteractable>();
        if (interactable == null) interactable = obj.GetComponentInParent<IInteractable>();
        if (interactable != null) {
            Debug.Log("[E] " + interactable.GetHintText());
            interactable.Interact();
            return true;
        }

        return false;
    }

    private void PickupToInventory(AbilityItem item) {
        item.OnPickup();
        item.transform.SetParent(handPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        _inventory.AddItem(item);
        item.gameObject.SetActive(false);

        PlayerAbilityManager abilityManager = Object.FindAnyObjectByType<PlayerAbilityManager>();
        if (abilityManager != null) {
            abilityManager.OnItemPickedUp();
            Debug.Log("[PlayerInteraction] B�ren-Manager �ber Pickup informiert!");
        }

        Debug.Log($"[PlayerInteraction] {item.AbilityName} erfolgreich im Inventar verstaut!");
    }

    private void Pickup(IPickable item, GameObject obj) {
        _carriedItem = item;
        item.OnPickup();
        obj.transform.SetParent(handPosition);
        Debug.Log("Aufgehoben: " + obj.name);
    }

    private void Drop() {

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance)) {
            IPlaceable placeable = hit.collider.GetComponent<IPlaceable>();
            PickableItem item = (_carriedItem as MonoBehaviour).GetComponent<PickableItem>();

            if (placeable != null && placeable.CanPlace(item)) {
                placeable.Place(item);
                _carriedItem = null;
                return;
            }
        }

        GameObject obj = (_carriedItem as MonoBehaviour).gameObject;
        obj.transform.SetParent(null);
        _carriedItem.OnDrop();
        _carriedItem = null;
    }
}