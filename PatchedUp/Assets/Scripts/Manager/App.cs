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
        }
        else {
            Destroy(gameObject);
        }

        foreach (var manager in this.managers) {
            this.typeToManager.Add(manager.GetType(), manager);
        }
    }

    private IEnumerator Start() {
        foreach (var manager in this.managers) {
            yield return this.StartCoroutine(manager.Init());
        }

        foreach (var manager in this.managers) {
            yield return this.StartCoroutine(manager.Load());
        }

        // You could do a SceneChange here if you want to load a scene after all managers are initialized and loaded.
        // e.g. Main Menu or something like that.
    }

    public T GetManager<T>() where T : class {
        if (this.typeToManager.ContainsKey(typeof(T))) {
            return this.typeToManager[typeof(T)] as T;
        }
        return null;
    }
}
