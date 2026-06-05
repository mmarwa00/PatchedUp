using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerAbilityManager : ManagerBase {
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventoryMenuUI inventoryMenuUI;

    [Header("Prefabs Database")]
    [SerializeField] private List<AbilityItem> allAvailableAbilityPrefabs;

    private StarterAssetsInputs _input;
    private Camera _mainCamera;
    private int _currentSelectedSlot = 0;

    public override IEnumerator Init() {
        _input = GetComponent<StarterAssetsInputs>();
        _mainCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<PlayerInventory>();

        Debug.Log("[PlayerAbilityManager] Initialisiert!");
        yield break;
    }

    public override IEnumerator Load() {
        // 1. Daten vom SaveSystem holen
        SaveData currentSave = SaveSystem.Load();

        if (currentSave != null && currentSave.collectedAbilityNames.Count > 0) {
            Debug.Log("[PlayerAbilityManager] Lade gespeicherte Fähigkeiten...");

            // Gespeicherte Fähigkeiten wieder ins Inventar spawnen
            foreach (string abilityName in currentSave.collectedAbilityNames) {
                AbilityItem matchingPrefab = allAvailableAbilityPrefabs.Find(p => p.AbilityName == abilityName);
                if (matchingPrefab != null) {
                    AbilityItem spawnedItem = Instantiate(matchingPrefab, transform);
                    playerInventory.AddItem(spawnedItem);
                }
            }
        }
        yield break;
    }

    private void Update() {
        HandleAbilityCycling();
        HandleAbilityUsage();


        if (inventoryMenuUI != null && inventoryMenuUI.IsMenuOpen) return;
    }
    private void Awake() {
        // Wir holen uns die Tasten direkt vom selben Objekt
        _input = GetComponent<StarterAssetsInputs>();

        if (playerInventory == null) {
            playerInventory = GetComponent<PlayerInventory>();
        }

        Debug.Log($"[PlayerAbilityManager] Physischer Awake-Kanal geöffnet! Hat Input gefunden: {_input != null}");
    }
    public void TriggerSaveGame() {
        // Aktuellen Zustand laden oder neuen erstellen
        SaveData dataToSave = SaveSystem.Load() ?? new SaveData();

        dataToSave.collectedAbilityNames.Clear();
        foreach (var item in playerInventory.ItemsInBag) {
            dataToSave.collectedAbilityNames.Add(item.AbilityName);
        }

        dataToSave.lastPlayedUtcTicks = System.DateTime.UtcNow.Ticks;

        SaveSystem.Save(dataToSave);
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
            Debug.Log($"[C] Fähigkeit gewechselt auf: {items[_currentSelectedSlot].AbilityName}");
        }
    }

    private void HandleAbilityUsage() {
        if (_input == null) return;

        if (_input.useAbility) {
            _input.useAbility = false;

            var items = playerInventory.ItemsInBag;
            if (items.Count == 0) {
                Debug.Log("Du hast keine Fähigkeit zum Einsetzen!");
                return;
            }

            if (items.Count > _currentSelectedSlot) {
                Debug.Log($"[F] Benutze Fähigkeit: {items[_currentSelectedSlot].AbilityName}");
                items[_currentSelectedSlot].UseAbility(_mainCamera);
            }
        }
    }
}