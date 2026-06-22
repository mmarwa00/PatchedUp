using System;
using System.IO;
using UnityEngine;

public static class SaveSystem {
    private const string SaveFileName = "save.json";
    private const int CurrentSaveVersion = 1;

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void Save(SaveData data) {
        if (data == null) {
            Debug.LogWarning("[SaveSystem] Tried to save null data � ignored.");
            return;
        }

        try {
            data.saveVersion = CurrentSaveVersion;
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Saved to {SavePath}");
        }
        catch (Exception e) {
            Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
        }
    }

    public static SaveData Load() {
        if (!File.Exists(SavePath)) {
            return null;
        }

        try {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            return Migrate(data);
        }
        catch (Exception e) {
            Debug.LogError($"[SaveSystem] Load failed, ignoring file: {e.Message}");
            return null;
        }
    }

    public static bool DeleteSave() {
        if (!File.Exists(SavePath)) {
            return false;
        }
        File.Delete(SavePath);
        Debug.Log($"[SaveSystem] Deleted save at {SavePath}");
        return true;
    }

    private static SaveData Migrate(SaveData data) {
        if (data == null) {
            return null;
        }
        if (data.saveVersion < CurrentSaveVersion) {
            data.saveVersion = CurrentSaveVersion;
        }
        return data;
    }
}
