using UnityEngine;
using System.Collections;

public class UIManager : ManagerBase
{
    [Header("Canvases / Panels")]
    [SerializeField] private GameObject caughtCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject youWonCanvas;
    [SerializeField] private GameObject pauseCanvas;
    public override IEnumerator Init() {
        Debug.Log("[UIManager] UI-Leitungen erfolgreich verlegt!");
        yield break;
    }

    public override IEnumerator Load() {
        yield break;
    }

    public void ShowCaughtScreen(bool state) {
        if (caughtCanvas != null) caughtCanvas.SetActive(state);
    }

    public void ShowGameOverScreen(bool state) {
        if (gameOverCanvas != null) gameOverCanvas.SetActive(state);
    }
    public void ShowYouWonScreen(bool state) {
        if (youWonCanvas != null) youWonCanvas.SetActive(state);
    }
    public void ShowPauseScreen(bool state) {
        if (pauseCanvas != null) pauseCanvas.SetActive(state);
    }
}