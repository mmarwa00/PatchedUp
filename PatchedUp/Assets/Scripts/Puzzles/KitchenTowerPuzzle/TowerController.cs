using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TowerController : MonoBehaviour
{
    [Header("Tower Objects")] 
    [SerializeField] private int towerId;
    [SerializeField] private int[] utensilOrder;
    private int[] utensilInputOrder;
    private int currentTowerSize = 0;
    private int totalTowerSize;

    private void Start()
    {
        totalTowerSize = utensilOrder.Length;
        utensilInputOrder = new int[totalTowerSize];
    }

    private void OnEnable()
    {
        KitchenTowerEvents.OnSnap += HandleSnapEvent;
    }

    private void OnDisable()
    {
        KitchenTowerEvents.OnSnap -= HandleSnapEvent;
    }

    private void HandleSnapEvent(bool isSnapped, int id)
    {
        if (isSnapped)
        {
            if (currentTowerSize >= totalTowerSize) return;
            utensilInputOrder[currentTowerSize] = id;
            currentTowerSize++;
        }
        else
        {
            if (currentTowerSize <= 0) return;
            currentTowerSize--;
            utensilInputOrder[currentTowerSize] = -1;
        }

        CheckIfCorrect();
    }

    private void CheckIfCorrect()
    {
        if (totalTowerSize != currentTowerSize) return;
        
        bool isEqual =  utensilInputOrder.SequenceEqual(utensilOrder);
        
        KitchenTowerEvents.OnTowerBuiltEvent(isEqual,  towerId);
        
    }
}
