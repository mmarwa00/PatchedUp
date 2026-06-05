using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour {
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject extrasMenu;
    private GameObject loading;

    public AudioSource buttonSound;



    void Start() {
        mainMenu = GameObject.Find("MainMenuCanvas");
        optionsMenu = GameObject.Find("OptionsCanvas");
        extrasMenu = GameObject.Find("ExtrasCanvas");
        loading = GameObject.Find("LoadingCanvas");

        mainMenu.GetComponent<Canvas>().enabled = true;
        optionsMenu.GetComponent<Canvas>().enabled = false;
        extrasMenu.GetComponent<Canvas>().enabled = false;
        loading.GetComponent<Canvas>().enabled = false;
    }

    public void StartButton() {
        loading.GetComponent<Canvas>().enabled = true;
        mainMenu.GetComponent<Canvas>().enabled = false;
        buttonSound.Play();
        SceneManager.LoadScene("GameScene");
    }

    public void OptionsButton() {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = false;
        optionsMenu.GetComponent<Canvas>().enabled = true;
    }

    public void ExtrasButton() {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = false;
        extrasMenu.GetComponent<Canvas>().enabled = true;
    }

    public void ExitGameButton() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


    public void ReturnToMainMenuButton() {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = true;
        optionsMenu.GetComponent<Canvas>().enabled = false;
        extrasMenu.GetComponent<Canvas>().enabled = false;
    }



    void Update() {

    }
}