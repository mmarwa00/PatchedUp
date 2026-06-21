using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Tower Setup")]
    [SerializeField] private int towerId;
    [SerializeField] private int[] utensilOrder;
    [SerializeField] private Transform stackAnchor;
    [SerializeField] private float stackSpacing = 0f;
    [SerializeField] private Transform snapZone;

    private struct StackEntry
    {
        public PickableItem item;
        public int utensilId;
        public float topY;
    }

    private readonly List<StackEntry> stack = new List<StackEntry>();
    private float baseTopY;
    private float currentTopY;
    private bool solved;

    private void Start()
    {
        if (stackAnchor == null) stackAnchor = transform;
        baseTopY = GetBaseTopY();
        currentTopY = baseTopY;
        UpdateSnapZonePosition();
    }
    
    private void UpdateSnapZonePosition()
    {
        if (snapZone == null) return;
        snapZone.position = new Vector3(stackAnchor.position.x, currentTopY, stackAnchor.position.z);
    }

    private float GetBaseTopY()
    {
        return TryGetWorldBounds(gameObject, out Bounds b) ? b.max.y : stackAnchor.position.y;
    }

    public bool CanPlace(PickableItem item)
    {
        if (solved || stack.Count >= utensilOrder.Length) return false;
        return item != null && item.GetComponent<KitchenUtensilSnapRegister>() != null;
    }

    public void Place(PickableItem item)
    {
        KitchenUtensilSnapRegister register = item.GetComponent<KitchenUtensilSnapRegister>();
        if (register == null) return;

        Transform t = item.transform;
        t.SetParent(transform);
        t.rotation = stackAnchor.rotation;
        
        float newTopY = currentTopY;
        if (TryGetWorldBounds(item.gameObject, out Bounds b))
        {
            t.position += new Vector3(
                stackAnchor.position.x - b.center.x,
                currentTopY - b.min.y,
                stackAnchor.position.z - b.center.z);
            newTopY = currentTopY + b.size.y + stackSpacing;
        }
        else
        {
            t.position = new Vector3(stackAnchor.position.x, currentTopY, stackAnchor.position.z);
        }
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // only the topmost item interactable
        if (stack.Count > 0) SetItemPickable(stack[stack.Count - 1].item, false);

        stack.Add(new StackEntry { item = item, utensilId = register.UtensilId, topY = newTopY });
        currentTopY = newTopY;
        UpdateSnapZonePosition();

        if (stack.Count == utensilOrder.Length) CheckIfCorrect();
    }

    private void CheckIfCorrect()
    {
        bool ok = stack.Select(e => e.utensilId).SequenceEqual(utensilOrder);
        KitchenTowerEvents.OnTowerBuiltEvent(ok, towerId);
        if (ok)
        {
            solved = true;
            LockStack(); // freeze the finished tower
        }
    }

    private void LockStack()
    {
        foreach (StackEntry entry in stack)
            SetItemPickable(entry.item, false);
        
        if (snapZone != null) snapZone.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (solved) return;

        bool popped = false;
        while (stack.Count > 0)
        {
            StackEntry top = stack[stack.Count - 1];
            if (top.item != null && top.item.transform.parent == transform) break;

            stack.RemoveAt(stack.Count - 1);
            currentTopY = stack.Count == 0 ? baseTopY : stack[stack.Count - 1].topY;
            popped = true;
        }

        // top becomes interactable again, and the snap zone drops back down to the new top
        if (popped)
        {
            if (stack.Count > 0) SetItemPickable(stack[stack.Count - 1].item, true);
            UpdateSnapZonePosition();
        }
    }
    
    private static void SetItemPickable(PickableItem item, bool pickable)
    {
        if (item == null) return;
        item.SetPickable(pickable);
    }
    
    // get center location
    private static bool TryGetWorldBounds(GameObject go, out Bounds bounds)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            return true;
        }

        Collider col = go.GetComponent<Collider>();
        if (col != null)
        {
            bounds = col.bounds;
            return true;
        }

        bounds = default;
        return false;
    }
}
