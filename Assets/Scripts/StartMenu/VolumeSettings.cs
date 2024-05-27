using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myAudio;
    [SerializeField] private Slider musicSlider;
    
    // Start is called before the first frame update
    private void Start(){
        if (PlayerPrefs.HasKey("musicVolume")){
            LoadVolume();
        }
        else{
            SetMusicVolume();
        }
       // SetMusicVolume();
    }
    public void SetMusicVolume(){
        float volume = musicSlider.value;
        myAudio.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void LoadVolume(){
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
}
