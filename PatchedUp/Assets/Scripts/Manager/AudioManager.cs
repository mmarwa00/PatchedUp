using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : ManagerBase {
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip introSceneMusic;
    [SerializeField] private AudioClip gameSceneMusic;

    public override IEnumerator Init() {
        Debug.Log("[AudioManager] Initialisiere Musik-Zentrale...");


        SceneManager.sceneLoaded += OnSceneLoaded;

        yield break;
    }

    public override IEnumerator Load() {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
        yield break;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"[AudioManager] Szene gewechselt: {scene.name}. Passe Musik an...");
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName) {
        AudioClip targetClip = null;


        if (sceneName == "MainMenu") {
            targetClip = mainMenuMusic;
        }
        else if (sceneName == "IntroScene") {
            targetClip = introSceneMusic;
        }
        else if (sceneName == "zone1_greyboxing")
        {
            targetClip = gameSceneMusic;
        }


        if (targetClip != null) {
            if (musicSource.clip == targetClip && musicSource.isPlaying) return;

            musicSource.Stop();
            musicSource.clip = targetClip;
            musicSource.loop = true; // Musik soll im Hintergrund immer loopen!
            musicSource.Play();
            Debug.Log($"[AudioManager] Spiele jetzt: {targetClip.name}");
        }
        else {
            musicSource.Stop();
        }
    }
}