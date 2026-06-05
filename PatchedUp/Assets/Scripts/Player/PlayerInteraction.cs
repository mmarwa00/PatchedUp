using UnityEngine;
using StarterAssets;

public class PlayerInteraction : MonoBehaviour {
    
    
    [SerializeField] private float interactionDistance = 3.0f;
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
    }

    private void Update() {
        if (_input.interact) {
            _input.interact = false;

            if (_carriedItem != null) {
                Drop();
                return;
            }

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance)) {

                AbilityItem abilityItem = hit.collider.GetComponent<AbilityItem>();
                if (abilityItem != null) {
                    PickupToInventory(abilityItem);
                    return;
                }

                IPickable pickable = hit.collider.GetComponent<IPickable>();
                if (pickable != null) {
                    Pickup(pickable, hit.collider.gameObject);
                    return;
                }

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) {
                    Debug.Log("[E] " + interactable.GetHintText());
                    interactable.Interact();
                }
            }
        }

        if (_carriedItem != null) {
            Transform itemTransform = (_carriedItem as MonoBehaviour).transform;
            itemTransform.position = handPosition.position;
            itemTransform.rotation = handPosition.rotation;
        }
    }

    private void PickupToInventory(AbilityItem item) {

        item.OnPickup();

        item.transform.SetParent(handPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        _inventory.AddItem(item);

        if (_inventory.ItemsInBag.Count == 1) { 
            currentEquippedIndex = 0;
            item.gameObject.SetActive(true);
        }
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