using UnityEngine;

public class InteractableCubeTest : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Interact() {
        // Das passiert, wenn wir E drücken!
        Debug.Log("Xylophon wurde gestartet! Musik läuft.");
    }

    public string GetHintText() {
        return "[E] Xylophon starten";
    }
}
