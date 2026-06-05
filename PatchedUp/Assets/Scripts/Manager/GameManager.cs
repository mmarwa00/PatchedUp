using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : ManagerBase
{
    private int _catchCount = 0;
    private bool _isPaused = false;

    public override IEnumerator Init() {
        Debug.Log("[GameManager] Erfolgreich im App-System initialisiert!");
        yield break;
    }

    public override IEnumerator Load() {
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

        //  physische Save-File auf der Festplatte gelöscht wird:
        // SaveSystem.DeleteSave(); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}