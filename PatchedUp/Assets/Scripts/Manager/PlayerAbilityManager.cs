using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerAbilityManager : ManagerBase {

    private PlayerInventory playerInventory;
    private InventoryMenuUI inventoryMenuUI;

    private GameObject bodyNormal;
    private GameObject bodyHook;
    private GameObject bodyPin;

    [Header("Prefabs Database")]
    [SerializeField] private List<AbilityItem> allAvailableAbilityPrefabs;

    private StarterAssetsInputs _input;
    private Camera _mainCamera;
    private int _currentSelectedSlot = -1;

    public void SetupBearBodies(GameObject normal, GameObject hook, GameObject pin) {
        bodyNormal = normal;
        bodyHook = hook;
        bodyPin = pin;

        if (_currentSelectedSlot != -1 && playerInventory.ItemsInBag.Count > _currentSelectedSlot) {
            UpdatePhysicalBearVisuals(playerInventory.ItemsInBag[_currentSelectedSlot].AbilityName);
        }
        else {
            UpdatePhysicalBearVisuals("Normal");
        }
        Debug.Log("[PlayerAbilityManager] Bären-Körper erfolgreich aus der Spielszene gekoppelt!");
    }

    public override IEnumerator Init() {
        if (_input == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                _input = player.GetComponent<StarterAssetsInputs>();
            }
        }
        _mainCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<PlayerInventory>();
        yield break;
    }

    public override IEnumerator Load() {
        float timeout = 2f;
        while ((bodyNormal == null || bodyHook == null || bodyPin == null) && timeout > 0f) {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (bodyNormal == null) {
            Debug.LogWarning("[PlayerAbilityManager] Bodies noch immer null nach Timeout!");
            yield break;
        }

        SaveData currentSave = SaveSystem.Load();
        if (currentSave != null && playerInventory != null) {
            if (playerInventory.ItemsInBag.Count > 0) {
                _currentSelectedSlot = 0;
                string activeAbility = playerInventory.ItemsInBag[_currentSelectedSlot].AbilityName;
                UpdatePhysicalBearVisuals(activeAbility);
                Debug.Log($"[PlayerAbilityManager] Aktivierte Fähigkeit: {activeAbility}");
            }
        }
        yield break;
    }
    private void Awake() {
        if (playerInventory == null) playerInventory = Object.FindAnyObjectByType<PlayerInventory>();

        if (_input == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                _input = player.GetComponent<StarterAssetsInputs>();
            }
        }

        if (inventoryMenuUI == null) inventoryMenuUI = Object.FindAnyObjectByType<InventoryMenuUI>();

        if (bodyPin == null) {
            Transform t = transform.Find("Bear_PIN@Idle");
            if (t != null) bodyPin = t.gameObject;
        }
        if (bodyHook == null) {
            Transform t = transform.Find("Teddy_idle");
            if (t != null) bodyHook = t.gameObject;
        }
        if (bodyNormal == null) {
            Transform t = transform.Find("Patched@Idle");
            if (t != null) bodyNormal = t.gameObject;
        }

        Debug.Log($"[PlayerAbilityManager] Physischer Körper-Check: Normal={bodyNormal != null}, Hook={bodyHook != null}, Pin={bodyPin != null}");
    }

    private void Update() {
        if (_input == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                _input = player.GetComponent<StarterAssetsInputs>();
                if (_input != null) {
                    Debug.Log("[PlayerAbilityManager] _input erfolgreich nachträglich gefunden!");
                }
            }
        }

        if (_mainCamera == null) {
            _mainCamera = Camera.main;
        }


        if (inventoryMenuUI != null && inventoryMenuUI.IsMenuOpen) return;
        if (_input == null || playerInventory == null) return;

        if (_input.switchAbility) {
            Debug.Log($"[DEBUG] C erkannt! switchAbility={_input.switchAbility}, items.Count={playerInventory.ItemsInBag.Count}, currentSlot={_currentSelectedSlot}, bodyHook={bodyHook != null}, bodyPin={bodyPin != null}, bodyNormal={bodyNormal != null}");
        }

        HandleAbilityCycling();
        HandleAbilityUsage();
    }
    public void SetupInventoryReferences(PlayerInventory inventory, InventoryMenuUI menuUI) {
        this.playerInventory = inventory;
        this.inventoryMenuUI = menuUI;
        Debug.Log("[PlayerAbilityManager] Inventar und Menü-UI erfolgreich aus der Spielszene gekoppelt!");
    }

    private void HandleAbilityCycling() {
        if (_input == null) return;

        if (_input.switchAbility) {
            _input.switchAbility = false;

            var items = playerInventory.ItemsInBag;
            if (items.Count <= 1) {
                Debug.Log("[PlayerAbilityManager] Keine weiteren Fähigkeiten zum Wechseln da.");
                return;
            }

            _currentSelectedSlot = (_currentSelectedSlot + 1) % items.Count;
            string activeAbility = items[_currentSelectedSlot].AbilityName;

            Debug.Log($"[PlayerAbilityManager]  Wechsel zu Bär mit: {activeAbility} (Slot: {_currentSelectedSlot})");
            UpdatePhysicalBearVisuals(activeAbility);
        }
    }

    private void HandleAbilityUsage() {
        if (_input == null) return;

        if (_input.useAbility) {
            _input.useAbility = false;

            var items = playerInventory.ItemsInBag;
            Debug.Log($"[USE-DEBUG] F gedrückt! items.Count={items.Count}, currentSlot={_currentSelectedSlot}");

            if (items.Count == 0 || _currentSelectedSlot == -1) {
                Debug.Log("Du hast keine Fähigkeit aktiv!");
                return;
            }

            if (items.Count > _currentSelectedSlot && items[_currentSelectedSlot] != null) {
                Debug.Log($"[USE-DEBUG] Rufe UseAbility auf für: {items[_currentSelectedSlot].AbilityName}, Typ: {items[_currentSelectedSlot].GetType().Name}");
                items[_currentSelectedSlot].UseAbility(_mainCamera);
            }
            else {
                Debug.Log($"[USE-DEBUG] Slot ungültig! items.Count={items.Count}, currentSlot={_currentSelectedSlot}");
            }
        }
    }

    private void UpdatePhysicalBearVisuals(string abilityName) {
        if (bodyNormal != null) bodyNormal.SetActive(false);
        if (bodyHook != null) bodyHook.SetActive(false);
        if (bodyPin != null) bodyPin.SetActive(false);

        if (abilityName == "Hook") {
            if (bodyHook != null) bodyHook.SetActive(true);
        }
        else if (abilityName == "Pin") {
            if (bodyPin != null) bodyPin.SetActive(true);
        }
        else {
            if (bodyNormal != null) bodyNormal.SetActive(true);
        }
    }

    public string GetCurrentAbilityName() {
        if (playerInventory == null || _currentSelectedSlot == -1) return "Normal";
        var items = playerInventory.ItemsInBag;
        if (items.Count > _currentSelectedSlot && items[_currentSelectedSlot] != null)
            return items[_currentSelectedSlot].AbilityName;
        return "Normal";
    }
    public void OnItemPickedUp() {
        var items = playerInventory.ItemsInBag;

        if (items.Count > 0) {
            if (_currentSelectedSlot == -1) {
                _currentSelectedSlot = 0;
            }
            else {
                _currentSelectedSlot = items.Count - 1;
            }

            string activeAbility = items[_currentSelectedSlot].AbilityName;
            Debug.Log($"[PlayerAbilityManager] neues Item aufgesammelt! Rüste sofort aus: {activeAbility}");
            UpdatePhysicalBearVisuals(activeAbility);
        }
    }
}