using UnityEngine;

public class BodyRegistrator : MonoBehaviour {
    [Header("Ziehe hier die 3 Bären-Modelle aus dieser Szene rein!")]
    [SerializeField] private GameObject bodyNormal;
    [SerializeField] private GameObject bodyHook;
    [SerializeField] private GameObject bodyPin;

    private void Start() {
        // Wir suchen die App im Arbeitsspeicher
        if (App.Instance != null) {
            PlayerAbilityManager abilityManager = App.Instance.GetManager<PlayerAbilityManager>();

            if (abilityManager != null) {
                // Wir übergeben dem Manager die 3 Körper
                abilityManager.SetupBearBodies(bodyNormal, bodyHook, bodyPin);
            }
            else {
                Debug.LogError("[Registrator] PlayerAbilityManager konnte nicht in App gefunden werden!");
            }
        }
        else {
            Debug.LogWarning("[Registrator] Keine App.Instance gefunden! (Passiert, wenn du die GameScene direkt startest ohne Hauptmenü)");
        }
    }
}