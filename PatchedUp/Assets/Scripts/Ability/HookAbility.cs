using UnityEngine;
using System.Collections;

public class HookAbility : AbilityItem {
    [Header("Hook Settings")]
    [SerializeField] private float hookRange = 8f;
    [SerializeField] private float pullSpeed = 8f;
    [SerializeField] private LayerMask pullableLayers;
    [SerializeField] private Transform handPosition;

    private static bool _isPulling = false;

    public override void UseAbility(Camera mainCamera) {
        if (_isPulling) return;

        Debug.Log($"[Hook] Feure Haken ab!");

        Collider[] hits = Physics.OverlapSphere(transform.position, hookRange, pullableLayers);

        if (hits.Length == 0) {
            Debug.Log("[Hook] Nichts in Reichweite.");
            return;
        }

        Collider closest = null;
        float closestDist = float.MaxValue;

        foreach (var col in hits) {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist) {
                closestDist = dist;
                closest = col;
            }
        }

        if (closest == null) return;

        Rigidbody targetRb = closest.GetComponent<Rigidbody>();
        if (targetRb != null) {
            Debug.Log($"[Hook] Objekt getroffen: {closest.name}! Ziehe zu mir...");

            CoroutineRunner.Instance.StartCoroutine(PullObject(targetRb));
        }
        else {
            Debug.Log("[Hook] Objekt hat keinen Rigidbody und kann nicht bewegt werden.");
        }
    }

    private IEnumerator PullObject(Rigidbody targetRb) {
        _isPulling = true;
        targetRb.useGravity = false;
        float stopDistance = 0.05f;

        while (targetRb != null && Vector3.Distance(targetRb.position,
            handPosition != null ? handPosition.position : transform.position) > stopDistance) {

            Vector3 targetPos = handPosition != null ? handPosition.position : transform.position;
            Vector3 direction = (targetPos - targetRb.position).normalized;
            targetRb.linearVelocity = direction * pullSpeed;
            yield return null;
        }

        if (targetRb != null) {
            targetRb.linearVelocity = Vector3.zero;
            targetRb.useGravity = true;
        }
        _isPulling = false;
    }
}