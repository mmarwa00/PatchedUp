using UnityEngine;

public class SnapPoint : MonoBehaviour, IPlaceable {
    [SerializeField] private string acceptedItemTag = "Utensil";
    private bool _isOccupied = false;

    public bool CanPlace(PickableItem item) {
        return !_isOccupied && item.CompareTag(acceptedItemTag);
    }

    public void Place(PickableItem item) {
        _isOccupied = true;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.OnDrop();
        Debug.Log("Platziert auf: " + gameObject.name);
    }
}