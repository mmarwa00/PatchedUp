using UnityEngine;
using System.Collections;

public class GlobalMusic : MonoBehaviour
{
    public static GlobalMusic Instance;
    public AudioSource source;
    public AudioClip cuteMusic;
    public AudioClip eerieMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}