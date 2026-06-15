using UnityEngine;
using StarterAssets;

public class PlayerInteraction : MonoBehaviour {

    [SerializeField] private float interactionDistance = 0.00005f;
    [SerializeField] private Transform handPosition;

    private StarterAssetsInputs _input;
    private PlayerInventory _inventory;
    private int currentEquippedIndex = 0;
    private IPickable _carriedItem;

    private void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        _inventory = GetComponent<PlayerInventory>();
    }

    private void Update() {
        // Reagiert auf das neue Input-System
        if (_input.pickUpAbility) {
            _input.pickUpAbility = false;

            if (_carriedItem != null) {
                Drop();
                return;
            }

            Debug.Log("[PlayerInteraction] [E] gedrückt! Suche Items in der Nähe...");

            // scannen eine unsichtbare Kugel um den Spieler herum
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);

            AbilityItem closestAbilityItem = null;
            float closestDistance = float.MaxValue;

            foreach (var col in hitColliders) {
                // Suchen, ob das getroffene Ding ein AbilityItem ist (auch in Eltern-Objekten)
                AbilityItem abilityItem = col.GetComponent<AbilityItem>();
                if (abilityItem == null) abilityItem = col.GetComponentInParent<AbilityItem>();

                if (abilityItem != null) {
                    // Wir berechnen den Abstand, um das allernächste Item zu schnappen
                    float dist = Vector3.Distance(transform.position, abilityItem.transform.position);
                    if (dist < closestDistance) {
                        closestDistance = dist;
                        closestAbilityItem = abilityItem;
                    }
                }
            }

            // Wenn wir ein AbilityItem im Umkreis gefunden haben, sammeln wir es auf!
            if (closestAbilityItem != null) {
                Debug.Log($"[PlayerInteraction] Item über Nähe gefunden: {closestAbilityItem.AbilityName}!");
                PickupToInventory(closestAbilityItem);
                return;
            }

            Debug.Log("[PlayerInteraction] Kein gültiges AbilityItem im Umkreis gefunden.");
        }

        if (_carriedItem != null) {
            Transform itemTransform = (_carriedItem as MonoBehaviour).transform;
            itemTransform.position = handPosition.position;
            itemTransform.rotation = handPosition.rotation;
        }
    }

    private void PickupToInventory(AbilityItem item) {
        Debug.Log($"[PlayerInteraction] Sammle Fähigkeit auf: {item.AbilityName}");

        item.OnPickup();

        _inventory.AddItem(item);

        item.transform.SetParent(handPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.gameObject.SetActive(false);

        PlayerAbilityManager abilityManager = Object.FindAnyObjectByType<PlayerAbilityManager>();
        if (abilityManager != null) {
            abilityManager.OnItemPickedUp();
            Debug.Log("[PlayerInteraction] Bären-Manager über Pickup informiert!");
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
        if (_carriedItem != null) {
            GameObject obj = (_carriedItem as MonoBehaviour).gameObject;
            obj.transform.SetParent(null);
            obj.transform.position = transform.position + transform.forward * 0.5f;
            _carriedItem.OnDrop();
            _carriedItem = null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}