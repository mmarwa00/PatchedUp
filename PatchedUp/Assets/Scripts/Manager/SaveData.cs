using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public int saveVersion = 1;
    public long lastPlayedUtcTicks;

    public int catchCount;
    public int solvedPuzzlesCount;

    public float posX;
    public float posY;
    public float posZ;

    public List<string> collectedAbilityNames = new List<string>();
}