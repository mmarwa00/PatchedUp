using UnityEngine;

public class GameOverLogic : MonoBehaviour {

    public void ClickedTryAgain() {
        if (App.Instance != null) {
            App.Instance.GetManager<GameManager>().RestartGame();
        }

    }
    public void ClickedQuit() {
        if (App.Instance != null) {
            App.Instance.GetManager<GameManager>().QuitGame();
        }
    }
}