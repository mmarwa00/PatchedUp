using System;
using UnityEngine;

public class KitchenUtensilSnapRegister : MonoBehaviour
{
    private Transform objectTopLocation;
    [SerializeField] private int utensilId;
    private bool isSnapped = false;

    private void Start()
    {
        objectTopLocation = this.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KitchenUtensil"))
        {
            Collider objCollider = other.gameObject.GetComponent<Collider>();
            float topY = objCollider.bounds.max.y;
            objectTopLocation.position = new Vector3(objectTopLocation.position.x, topY, objectTopLocation.position.z);
            isSnapped = true;
            KitchenTowerEvents.OnSnapEvent(isSnapped, utensilId);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KitchenUtensil"))
        {
            isSnapped = false;
            KitchenTowerEvents.OnSnapEvent(isSnapped, utensilId);
        }
    }
}
