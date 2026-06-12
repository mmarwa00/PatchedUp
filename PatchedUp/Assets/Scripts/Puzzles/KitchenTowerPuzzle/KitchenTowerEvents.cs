using System;

public class KitchenTowerEvents
{
    public static Action<bool, int> OnSnap;
    public static Action<bool, int> OnTowerBuilt;

    public static void OnSnapEvent(bool snapState, int id)
    {
        OnSnap?.Invoke(snapState, id);
    }

    public static void OnTowerBuiltEvent(bool isBuilt, int towerId)
    {
        OnTowerBuilt?.Invoke(isBuilt, towerId);
    }
}
