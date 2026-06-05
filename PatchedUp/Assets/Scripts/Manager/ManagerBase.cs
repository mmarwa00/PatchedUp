using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class ManagerBase : MonoBehaviour {
    public abstract IEnumerator Init();
    public abstract IEnumerator Load();
}