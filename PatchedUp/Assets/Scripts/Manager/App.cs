using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour {

    public ManagerBase[] managers;

    private Dictionary<Type, ManagerBase> typeToManager = new Dictionary<Type, ManagerBase>();

    private static App instance = null;

    public static App Instance {
        get {
            return App.instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            App.instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("App created, loading managers...");


            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else {
            Destroy(gameObject);
            return; 
        }

        foreach (var manager in this.managers) {
            this.typeToManager.Add(manager.GetType(), manager);
        }
    }

    private IEnumerator Start() {
        foreach (var manager in this.managers) {
            yield return this.StartCoroutine(manager.Init());
        }

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"[App] Szene erfolgreich gewechselt! Aktuelle Szene: {scene.name}");

        if (scene.name == "MainMenu") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        GameManager gm = App.Instance.GetManager<GameManager>();
        if (gm != null) gm.FinishRestart();

        if (MainMenuLogic.IsLoadingFromSave) {
            MainMenuLogic.IsLoadingFromSave = false;

            Debug.Log("[App] 'Continue'-Signal erkannt! Zwinge alle Manager zum Laden der Daten...");
            StartCoroutine(LoadAllManagersRoutine());
        }
        else if (scene.name != "MainMenu" && SaveSystem.Load() != null) {
            Debug.Log("[App] Save gefunden, lade Manager automatisch...");
            StartCoroutine(LoadAllManagersRoutine());
        }
    }

    private IEnumerator LoadAllManagersRoutine() {
        yield return null;
        yield return null; 

        foreach (var manager in this.managers) {
            if (manager != null) {
                Debug.Log($"[App] Starte Load für: {manager.GetType().Name}");
                yield return this.StartCoroutine(manager.Load());
            }
        }
    }

    private void OnDestroy() {
        if (instance == this) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public T GetManager<T>() where T : class {
        if (this.typeToManager.ContainsKey(typeof(T))) {
            return this.typeToManager[typeof(T)] as T;
        }
        return null;
    }
}