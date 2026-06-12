using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuLogic : MonoBehaviour {
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject extrasMenu;
    private GameObject loading;

    [SerializeField] private Button continueButton;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    public AudioSource buttonSound;



    void Start() {
        buttonSound.outputAudioMixerGroup = audioMixerGroup;
        
        mainMenu = GameObject.Find("MainMenuCanvas");
        optionsMenu = GameObject.Find("OptionsCanvas");
        extrasMenu = GameObject.Find("ExtrasCanvas");
        loading = GameObject.Find("LoadingCanvas");

        mainMenu.GetComponent<Canvas>().enabled = true;
        optionsMenu.GetComponent<Canvas>().enabled = false;
        extrasMenu.GetComponent<Canvas>().enabled = false;
        loading.GetComponent<Canvas>().enabled = false;

        if (continueButton != null) {
            bool hasSaveGame = SaveSystem.Load() != null;
            continueButton.interactable = hasSaveGame;
        }
    }

    public void StartButton() {
        
        buttonSound.Play();
        
        SaveSystem.DeleteSave();
        
        loading.GetComponent<Canvas>().enabled = true;
        mainMenu.GetComponent<Canvas>().enabled = false;
        
        SceneManager.LoadScene("IntroScene");
    }

    public void ContinueButton() {
        if (SaveSystem.Load() == null) return;

        buttonSound.Play();
        loading.GetComponent<Canvas>().enabled = true;
        mainMenu.GetComponent<Canvas>().enabled = false;

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