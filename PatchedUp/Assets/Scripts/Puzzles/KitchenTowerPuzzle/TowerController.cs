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
            utensilInputOrder[currentTowerSize] = id;
            currentTowerSize++;
        }
        else
        {
            utensilInputOrder[currentTowerSize] = -1;
            currentTowerSize--;
        }

        CheckIfCorrect();
    }

    private void CheckIfCorrect()
    {
        if (totalTowerSize != currentTowerSize) return;
        
        bool isEqual =  utensilInputOrder.SequenceEqual(utensilOrder);
        
        KitchenTowerEvents.OnTowerBuilt(isEqual,  towerId);
        
    }
}
