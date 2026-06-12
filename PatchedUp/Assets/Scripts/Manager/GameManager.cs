using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : ManagerBase
{
    private int _catchCount = 0;
    private bool _isPaused = false;

    public override IEnumerator Init() {
        Debug.Log("[GameManager] Erfolgreich im App-System initialisiert!");
        yield break;
    }

    public override IEnumerator Load() {
        Debug.Log("[GameManager] Startet automatischen Ladevorgang aus der save.json...");

        SaveData loadedData = SaveSystem.Load();

        // Wenn keine Datei existiert
        if (loadedData == null) {
            Debug.Log("[GameManager] Keine save.json gefunden oder Neues Spiel gestartet. Nutze Standard-Startwerte.");
            yield break;
        }

        this._catchCount = loadedData.catchCount;
        Debug.Log($"[GameManager] Geladener Catch-Count: {this._catchCount}");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {

            // Controller kurz ausschalten, ihn teleportieren, und wieder einschalten!
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = new Vector3(loadedData.posX, loadedData.posY, loadedData.posZ);
            Debug.Log($"[GameManager] Spieler erfolgreich teleportiert nach X: {loadedData.posX}, Y: {loadedData.posY}");

            if (controller != null) controller.enabled = true;
        }

        PlayerAbilityManager abilityManager = App.Instance.GetManager<PlayerAbilityManager>();
        if (abilityManager != null) {
            PlayerInventory inventory = abilityManager.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.ItemsInBag != null) {
                inventory.ItemsInBag.Clear();


                foreach (string abilityName in loadedData.collectedAbilityNames) {
                    // Beispiel: inventory.AddLoadedItemByName(abilityName);
                }
            }
        }

        yield break;
    }

    private void Update() {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
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
        _catchCount++;

        if (_catchCount == 1) {
            Debug.Log("[GameManager] Erster Hit! Gliedmaßenverlust eingeleitet.");
            player.ApplySpeedPenalty(0.5f);
            player.StartRespawnSequence();
        }
        else if (_catchCount >= 2) {
            Debug.Log("[GameManager] Zweiter Hit! Game Over.");
            player.FreezePlayer();

            // UIManager über die App-Zentrale rufen!
            App.Instance.GetManager<UIManager>().ShowGameOverScreen(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PlayerWon(CaptureSystem player) {
        Debug.Log("[GameManager] Freiheit erreicht! Spieler hat gewonnen.");
        player.FreezePlayer();

        App.Instance.GetManager<UIManager>().ShowYouWonScreen(true);

        // Maus freigeben (falls man doch schneller klicken will)
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        // Unity wartet exakt 10.0f Sekunden und ruft dann automatisch "QuitGame" auf!
        Invoke("QuitGame", 10.0f);
    }

    public void RestartGame() {

        _catchCount = 0;

        PlayerAbilityManager abilityManager = App.Instance.GetManager<PlayerAbilityManager>();
        if (abilityManager != null) {

            PlayerInventory inventory = abilityManager.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.ItemsInBag != null) {
                inventory.ItemsInBag.Clear();
            }
        }

        SaveSystem.DeleteSave(); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGame() {
        Debug.Log("[GameManager] Starte Speichervorgang...");

        SaveData dataToSave = new SaveData();
        dataToSave.lastPlayedUtcTicks = DateTime.UtcNow.Ticks;

        dataToSave.catchCount = this._catchCount;

        // Wie viele Puzzles gelöst wurden (falls du eine Variable dafür hast)
        // dataToSave.solvedPuzzlesCount = this._solvedPuzzlesCount;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            Vector3 currentPos = player.transform.position;
            dataToSave.posX = currentPos.x;
            dataToSave.posY = currentPos.y;
            dataToSave.posZ = currentPos.z;
            Debug.Log($"[GameManager] Speicher Position: X:{dataToSave.posX}, Y:{dataToSave.posY}");
        }

        PlayerAbilityManager abilityManager = App.Instance.GetManager<PlayerAbilityManager>();
        if (abilityManager != null) {
            PlayerInventory inventory = abilityManager.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.ItemsInBag != null) {

                dataToSave.collectedAbilityNames.Clear();

                foreach (AbilityItem item in inventory.ItemsInBag) {
                    if (item != null) {
                        dataToSave.collectedAbilityNames.Add(item.AbilityName);
                    }
                }
            }
        }

        SaveSystem.Save(dataToSave);
        // Druckt den exakten Pfad als anklickbaren Link in die Unity-Console!
        Debug.Log("[GameManager] Die Datei liegt HIER: " + Application.persistentDataPath);

    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}