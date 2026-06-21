using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TowerSnapZone : MonoBehaviour
{
    [SerializeField] private TowerController tower;

    private void OnTriggerStay(Collider other)
    {
        if (tower == null) return;

        PickableItem item = other.GetComponentInParent<PickableItem>();
        if (item == null) return;

        // only release item, carried and already placed kinematic/ignored.
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) return;

        if (tower.CanPlace(item)) tower.Place(item);
    }
}
