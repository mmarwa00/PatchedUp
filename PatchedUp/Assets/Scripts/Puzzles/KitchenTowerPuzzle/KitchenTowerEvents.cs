using System;

public class KitchenTowerEvents
{
    public static Action<bool, int> OnTowerBuilt;
    
    public static Action OnPuzzleSolved;

    public static void OnTowerBuiltEvent(bool isBuilt, int towerId)
    {
        OnTowerBuilt?.Invoke(isBuilt, towerId);
    }

    public static void OnPuzzleSolvedEvent()
    {
        OnPuzzleSolved?.Invoke();
    }
}
