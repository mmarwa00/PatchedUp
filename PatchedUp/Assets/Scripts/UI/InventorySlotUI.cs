using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotSUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI inventoryItemText;
    [SerializeField] private Image inventoryItemIcon;

    public void ClearSlot() {
        if (inventoryItemText != null) {
            inventoryItemText.text = "???";
            inventoryItemText.color = new Color(0, 0, 0, 0.5f);
        }

        if (inventoryItemIcon != null) {
            inventoryItemIcon.gameObject.SetActive(false);
        }
    }

    public void SetItemSlot(AbilityItem item) {
        if (item == null) {
            ClearSlot();
            return;
        }

        if (inventoryItemText != null) {
            inventoryItemText.text = item.AbilityName;
            inventoryItemText.color = Color.black;
        }

        if (inventoryItemIcon != null) {
            if (item.ItemIcon != null) {
                inventoryItemIcon.sprite = item.ItemIcon;
                inventoryItemIcon.gameObject.SetActive(true);
            }
            else {
                inventoryItemIcon.gameObject.SetActive(false);
            }
        }
    }
}