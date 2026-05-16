using UnityEngine;

public class TeaPlacement : MonoBehaviour, IPlaceable {
    [SerializeField] private Transform[] pentagramPoints; // 5 Zielpunkte
    [SerializeField] private float snapRadius = 0.3f;
    private int _placedCount = 0;

    public bool CanPlace(PickableItem item) {
        return GetNearestFreePoint(item.transform.position) != null;
    }

    public void Place(PickableItem item) {
        Transform point = GetNearestFreePoint(item.transform.position);
        if (point == null) return;

        item.transform.SetParent(point);
        item.transform.localPosition = Vector3.zero;
        item.OnDrop();

        _placedCount++;
        if (_placedCount >= pentagramPoints.Length) {
            Debug.Log("Pentagramm komplett! Puzzle gelöst.");
            // Hier später Reward-Event feuern
        }
    }

    private Transform GetNearestFreePoint(Vector3 position) {
        foreach (Transform point in pentagramPoints) {
            if (point.childCount == 0 &&
                Vector3.Distance(position, point.position) < snapRadius) {
                return point;
            }
        }
        return null;
    }
}