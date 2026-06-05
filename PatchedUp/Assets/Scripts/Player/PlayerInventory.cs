using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    [SerializeField] private List<AbilityItem> itemsInBag = new List<AbilityItem>();

    public List<AbilityItem> ItemsInBag => itemsInBag;

    public void AddItem(AbilityItem item) {
        if (!itemsInBag.Contains(item)) {
            itemsInBag.Add(item);
            item.gameObject.SetActive(false);
            Debug.Log($"Saved {item.AbilityName} to inventory.");
        }
    }

    public bool HasItem(string itemName) {
        foreach (var item in itemsInBag) {
            if (item.AbilityName == itemName) return true;
        }
        return false;
    }
}