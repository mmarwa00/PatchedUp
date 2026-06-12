using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetSfxVolume();
            SetMusicVolume();
            SetUIVolume();
        }
        
        
    }

    public void SetMasterVolume()
    {
        float volume = masterVolumeSlider.value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetSfxVolume()
    {
        float volume = sfxVolumeSlider.value;
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetUIVolume()
    {
        float volume = uiVolumeSlider.value;
        mixer.SetFloat("UIVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("UIVolume", volume);
    }

    private void LoadVolume()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        uiVolumeSlider.value = PlayerPrefs.GetFloat("UIVolume");
        
        SetMasterVolume();
        SetSfxVolume();
        SetMusicVolume();
        SetUIVolume();
    }
}
