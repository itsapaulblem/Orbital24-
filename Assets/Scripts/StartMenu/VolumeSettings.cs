using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    // references to the AudioMixer and Sliders for music and SFX volume 
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // check if volume settings are already saved in PlayerPrefs 
        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("sfxVolume"))
        {
            // Load saved volume settings 
            LoadVolume();
        }
        else
        {
            // Set default volume settings 
            SetMusicVolume();
            SetSFXVolume();
        }

        // Ensure sliders update volumes in real-time
        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
     //   sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    // method to set the music volume based on the music slider volume 
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        // convert the slider value to a logarithmic scale for volume adjustment 
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        // save the music volume setting in PlayerPrefs 
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    // method to load saved volume settings 
    private void LoadVolume()
    {
        if (musicSlider != null)
        {
            // load the saved music volume setting and update the slider value 
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            SetMusicVolume();
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            SetSFXVolume();
        }
    }
}
