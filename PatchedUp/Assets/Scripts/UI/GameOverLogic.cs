using UnityEngine;

public class GameOverLogic : MonoBehaviour {

    public void ClickedTryAgain() {
        Debug.Log("### TRY AGAIN WURDE KLASSISCH GEKLICKT! ###");
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