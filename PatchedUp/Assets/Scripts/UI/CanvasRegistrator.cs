using UnityEngine;

public class CanvasRegistrator : MonoBehaviour
{
    [Header("UI Canvases aus dieser Szene")]
    [SerializeField] private GameObject caughtCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject youWonCanvas;
    [SerializeField] private GameObject pauseCanvas;

    [Header("Spieler-Referenzen aus dieser Szene")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventoryMenuUI inventoryMenuUI;

    private void Start() {

        if (App.Instance != null) {
            UIManager uiManager = App.Instance.GetManager<UIManager>();
            if (uiManager != null) {
                uiManager.SetupGameCanvases(caughtCanvas, gameOverCanvas, youWonCanvas, pauseCanvas);
            }

            PlayerAbilityManager abilityManager = App.Instance.GetManager<PlayerAbilityManager>();
            if (abilityManager != null) {

                abilityManager.SetupInventoryReferences(playerInventory, inventoryMenuUI);
            }
        }
        else {
            Debug.LogWarning("[Registrator] Keine App gefunden. Startest du direkt in der Spielszene?");
        }
    }

}
