using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public int saveVersion = 1;
    public int highScore;
    public long lastPlayedUtcTicks;

    public List<string> collectedAbilityNames = new List<string>();
}
