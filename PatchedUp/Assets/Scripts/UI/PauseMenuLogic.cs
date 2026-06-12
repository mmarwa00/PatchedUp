using UnityEngine;

public class PauseMenuLogic : MonoBehaviour
{
    public void ClickedResume() {
        if (App.Instance != null) {
            App.Instance.GetManager<GameManager>().ResumeGame();
        }
    }

    public void ClickedSaveGame() {
        if (App.Instance != null) {
            App.Instance.GetManager<GameManager>().SaveGame();
            Debug.Log("[PauseMenu] Speicherbefehl erfolgreich an GameManager gesendet!");
        }
    }

    public void ClickedQuit() {
        if (App.Instance != null) {
            App.Instance.GetManager<GameManager>().QuitGame();
        }
    }
}
