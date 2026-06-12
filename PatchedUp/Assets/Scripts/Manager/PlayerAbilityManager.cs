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
        _input = GetComponent<StarterAssetsInputs>();
        _mainCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<PlayerInventory>();
        yield break;
    }

    public override IEnumerator Load() {
        SaveData currentSave = SaveSystem.Load();
        if (currentSave != null && currentSave.collectedAbilityNames.Count > 0) {
            foreach (string abilityName in currentSave.collectedAbilityNames) {
                AbilityItem matchingPrefab = allAvailableAbilityPrefabs.Find(p => p.AbilityName == abilityName);
                if (matchingPrefab != null) {
                    AbilityItem spawnedItem = Instantiate(matchingPrefab, transform);
                    playerInventory.AddItem(spawnedItem);
                }
            }
            if (playerInventory.ItemsInBag.Count > 0) {
                _currentSelectedSlot = 0;
            }
        }
        yield break;
    }

    private void Awake() {
        _input = GetComponent<StarterAssetsInputs>();
        if (playerInventory == null) playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update() {
        HandleAbilityCycling();
        HandleAbilityUsage();
        if (inventoryMenuUI != null && inventoryMenuUI.IsMenuOpen) return;
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
                Debug.Log("Keine weiteren Fähigkeiten zum Wechseln da.");
                return;
            }

            _currentSelectedSlot = (_currentSelectedSlot + 1) % items.Count;
            string activeAbility = items[_currentSelectedSlot].AbilityName;

            Debug.Log($"[C] Wechsel zu Bär mit: {activeAbility}");
            UpdatePhysicalBearVisuals(activeAbility);
        }
    }

    private void HandleAbilityUsage() {
        if (_input == null) return;

        if (_input.useAbility) {
            _input.useAbility = false;

            var items = playerInventory.ItemsInBag;
            if (items.Count == 0 || _currentSelectedSlot == -1) {
                Debug.Log("Du hast keine Fähigkeit aktiv!");
                return;
            }

            if (items.Count > _currentSelectedSlot && items[_currentSelectedSlot] != null) {
                items[_currentSelectedSlot].UseAbility(_mainCamera);
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

    public void OnItemPickedUp() {
        if (_currentSelectedSlot == -1 && playerInventory.ItemsInBag.Count > 0) {
            _currentSelectedSlot = 0;
            if (bodyNormal != null) { // Falls Körper schon registriert sind, direkt updaten
                UpdatePhysicalBearVisuals(playerInventory.ItemsInBag[_currentSelectedSlot].AbilityName);
            }
        }
    }
}