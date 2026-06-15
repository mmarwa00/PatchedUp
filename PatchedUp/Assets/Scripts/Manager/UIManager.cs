using UnityEngine;
using System.Collections;

public class UIManager : ManagerBase
{

    private GameObject caughtCanvas;
    private GameObject gameOverCanvas;
    private GameObject youWonCanvas;
    private GameObject pauseCanvas;
    public override IEnumerator Init() {
        Debug.Log("[UIManager] UI-Leitungen erfolgreich verlegt!");
        yield break;
    }

    public override IEnumerator Load() {
        yield break;
    }

    public void SetupGameCanvases(GameObject caught, GameObject gameOver, GameObject youWon, GameObject pause) {
        this.caughtCanvas = caught;
        this.gameOverCanvas = gameOver;
        this.youWonCanvas = youWon;
        this.pauseCanvas = pause;

        // Szenenstart erst mal alle canvas unsichtbar machen
        if (this.caughtCanvas != null) this.caughtCanvas.SetActive(false);
        if (this.gameOverCanvas != null) this.gameOverCanvas.SetActive(false);
        if (this.youWonCanvas != null) this.youWonCanvas.SetActive(false);
        if (this.pauseCanvas != null) this.pauseCanvas.SetActive(false);

        Debug.Log("[UIManager] Sämtliche Spiel-Canvases wurden erfolgreich mit der App gekoppelt!");
    }

    public void ShowCaughtScreen(bool state) {
        if (caughtCanvas != null) caughtCanvas.SetActive(state);
    }

    public void ShowGameOverScreen(bool state) {
        if (gameOverCanvas != null) {
            gameOverCanvas.SetActive(state);
        }

        if (state) {
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            var playerInputs = UnityEngine.Object.FindAnyObjectByType<StarterAssets.StarterAssetsInputs>();
            if (playerInputs != null) {
                playerInputs.cursorInputForLook = false;
                playerInputs.cursorLocked = false;
            }

            Debug.Log("[UIManager] Game-Over geöffnet. Mauszeiger rigoros für UI freigegeben!");
        }
        else {
            // Wenn das Menü geschlossen wird (z.B. beim Restart), Zeit wieder laufen lassen
            Time.timeScale = 1f;
        }
    }
    public void ShowYouWonScreen(bool state) {
        if (youWonCanvas != null) youWonCanvas.SetActive(state);
    }
    public void ShowPauseScreen(bool state) {
        if (pauseCanvas != null) pauseCanvas.SetActive(state);
    }
}