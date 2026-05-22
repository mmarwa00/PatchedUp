using System;

public class TeaCupPuzzleEvents
{
    public static Action<bool, int> OnSnap;

    public static void OnSnapEvent(bool snapState, int id)
    {
        OnSnap?.Invoke(snapState, id);
    }
}
