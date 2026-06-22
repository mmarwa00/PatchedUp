using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : ManagerBase {
    private int _catchCount = 0;
    private bool _isPaused = false;
    private bool _isRestarting;

    public bool IsPaused => _isPaused;

    public override IEnumerator Init() {
        Debug.Log("[GameManager] Erfolgreich im App-System initialisiert!");
        yield break;
    }

    public override IEnumerator Load() {
        Debug.Log("[GameManager] Startet automatischen Ladevorgang aus der save.json...");

        SaveData loadedData = SaveSystem.Load();

        if (loadedData == null) {
            Debug.Log("[GameManager] Keine save.json gefunden. Nutze Standard-Startwerte.");
            yield break;
        }

        this._catchCount = loadedData.catchCount;


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = new Vector3(loadedData.posX, loadedData.posY, loadedData.posZ);
            Debug.Log($"[GameManager] Spieler erfolgreich teleportiert nach X: {loadedData.posX}, Y: {loadedData.posY}");

            if (controller != null) controller.enabled = true;
        }

        // Wir warten einen winzigen Frame, damit in der Welt alles bereitsteht
        yield return null;


        if (loadedData.collectedAbilityNames != null && loadedData.collectedAbilityNames.Count > 0 && player != null) {

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            if (inventory == null) {
                PlayerAbilityManager abMan = player.GetComponent<PlayerAbilityManager>();
                if (abMan != null) inventory = abMan.GetComponent<PlayerInventory>();
            }

            AbilityItem[] itemsInWorld = UnityEngine.Object.FindObjectsByType<AbilityItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (AbilityItem item in itemsInWorld) {
                if (item.transform.parent != player.transform && !item.transform.IsChildOf(player.transform)) {

                    if (loadedData.collectedAbilityNames.Contains(item.AbilityName)) {

                        if (inventory != null && inventory.ItemsInBag != null) {
                            if (!inventory.ItemsInBag.Contains(item)) {
                                inventory.AddItem(item); // Item im Code-Inventar registrieren!
                                Debug.Log($"[GameManager] '{item.AbilityName}' erfolgreich ins Spieler-Inventar zurückgelegt!");
                            }
                        }

                        item.transform.SetParent(player.transform);
                        item.gameObject.SetActive(false);
                        Debug.Log($"[GameManager]  '{item.AbilityName}' vom Boden entfernt.");
                    }
                }
            }

            PlayerAbilityManager abilityManager = player.GetComponent<PlayerAbilityManager>();
            if (abilityManager == null) abilityManager = UnityEngine.Object.FindAnyObjectByType<PlayerAbilityManager>();

            if (abilityManager != null) {
                abilityManager.OnItemPickedUp();
                Debug.Log("[GameManager] PlayerAbilityManager erfolgreich synchronisiert!");
            }
        }


        _isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[GameManager] Mauszeiger erfolgreich gesperrt und versteckt!");

        yield break;
    }

    private void Update() {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            TogglePause();
        }
    }

    public void TogglePause() {
        if (_isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame() {
        _isPaused = true;
        Time.timeScale = 0f;

        UIManager ui = App.Instance.GetManager<UIManager>();
        if (ui != null) ui.ShowPauseScreen(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame() {
        _isPaused = false;
        Time.timeScale = 1f;

        UIManager ui = App.Instance.GetManager<UIManager>();
        if (ui != null) ui.ShowPauseScreen(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerCaught(CaptureSystem player) {
        if (_isRestarting) return;
        _catchCount++;

        if (_catchCount == 1) {
            Debug.Log("[GameManager] Erster Hit! Gliedmaßenverlust eingeleitet.");
            player.ApplySpeedPenalty(0.5f);
            player.StartRespawnSequence();
        }
        else if (_catchCount >= 2) {
            Debug.Log("[GameManager] Zweiter Hit! Game Over.");
            player.FreezePlayer();

            App.Instance.GetManager<UIManager>().ShowGameOverScreen(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PlayerWon(CaptureSystem player) {
        Debug.Log("[GameManager] Freiheit erreicht! Spieler hat gewonnen.");
        player.FreezePlayer();

        App.Instance.GetManager<UIManager>().ShowYouWonScreen(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Invoke("QuitGame", 10.0f);
    }

    public void RestartGame() {
        Time.timeScale = 1f;
        _catchCount = 0;
        _isRestarting = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null) inventory.ItemsInBag.Clear();

            CaptureSystem capture = player.GetComponent<CaptureSystem>();
            if (capture != null) capture.ResetState();
        }

        SaveSystem.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGame() {
        Debug.Log("[GameManager] Starte Speichervorgang...");

        SaveData dataToSave = new SaveData();
        dataToSave.lastPlayedUtcTicks = DateTime.UtcNow.Ticks;
        dataToSave.catchCount = this._catchCount;

        PuzzleManager puzzleManager = App.Instance.GetManager<PuzzleManager>();
        if (puzzleManager != null) {
            dataToSave.solvedPuzzleIds = puzzleManager.GetSolvedPuzzleIds();
            dataToSave.solvedPuzzlesCount = puzzleManager.GetCompletedPuzzleCount();
            Debug.Log($"[GameManager] {dataToSave.solvedPuzzlesCount} gelöste Puzzles gespeichert!");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            Vector3 currentPos = player.transform.position;
            dataToSave.posX = currentPos.x;
            dataToSave.posY = currentPos.y;
            dataToSave.posZ = currentPos.z;
            Debug.Log($"[GameManager] Speicher Position: X:{dataToSave.posX}, Y:{dataToSave.posY}");

            // Inventory direkt vom Player holen
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.ItemsInBag != null) {
                dataToSave.collectedAbilityNames.Clear();
                foreach (AbilityItem item in inventory.ItemsInBag) {
                    if (item != null) {
                        dataToSave.collectedAbilityNames.Add(item.AbilityName);
                        Debug.Log($"[GameManager] Speichere Item: {item.AbilityName}");
                    }
                }
                Debug.Log($"[GameManager] {dataToSave.collectedAbilityNames.Count} Items gespeichert!");
            }
            else {
                Debug.LogWarning("[GameManager] Kein PlayerInventory auf Player gefunden!");
            }
        }

        SaveSystem.Save(dataToSave);
    }

    public void FinishRestart() {
        _isRestarting = false;
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}