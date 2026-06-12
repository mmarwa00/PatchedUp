using UnityEngine;

public class HookAbility : AbilityItem {
    [Header("Hook Settings")]
    [SerializeField] private float hookRange = 15f;
    [SerializeField] private float pullForce = 12f;
    [SerializeField] private LayerMask pullableLayers;

    public override void UseAbility(Camera mainCamera) {
        if (mainCamera == null) return;

        Debug.Log($"[Hook] Feure Haken ab!");

        // Strahl aus der Bildschirmmitte schießen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, hookRange, pullableLayers)) {
            // Prüfen, ob das getroffene Objekt physikalisch bewegt werden kann
            Rigidbody targetRb = hit.collider.GetComponent<Rigidbody>();

            if (targetRb != null) {
                Debug.Log($"[Hook] Objekt getroffen: {hit.collider.name}! Ziehe zu mir...");

                // Wir nehmen die Kameraposition als Zielpunkt
                Vector3 pullDirection = (mainCamera.transform.position - hit.collider.transform.position).normalized;

                // Leicht nach oben ziehen, damit das Objekt nicht am Boden schleift/hakt
                pullDirection.y += 0.2f;
                pullDirection = pullDirection.normalized;

                // Wir setzen die aktuelle Geschwindigkeit kurz zurück, damit der Zug sofort knackig wirkt
                targetRb.linearVelocity = Vector3.zero;
                targetRb.AddForce(pullDirection * pullForce, ForceMode.Impulse);
            }
            else {
                Debug.Log("[Hook] Objekt hat keinen Rigidbody und kann nicht bewegt werden.");
            }
        }
        else {
            Debug.Log("[Hook] Nichts in Reichweite getroffen.");
        }
    }
}