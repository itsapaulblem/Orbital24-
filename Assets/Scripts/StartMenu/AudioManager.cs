using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource; 

    [Header("----- Audio Clip -----")]
    public AudioClip startBackground;
    public AudioClip gameBackground;
    public AudioClip enemybeingshot; 
    public AudioClip bobbeingshot; 
    public AudioClip bobshooting; 

    public static AudioManager Instance;
    private enum MusicState { Start, Game }
    private MusicState musicState;
    private string[] startScenes = { "Start" , "RegisterMenu" };

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        } else {
            Destroy(gameObject);
        }
    }
    private  void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (Array.IndexOf(startScenes, sceneName) > -1) {
            musicState = MusicState.Start;
            musicSource.clip = startBackground;
            musicSource.Play(); 
        } else {
            musicState = MusicState.Game;
            musicSource.clip = gameBackground;
            musicSource.Play();
        }
        //_audioSource.Play();
    }

    public void PlayMusic()
    {
        //if (_audioSource.isPlaying) return;
        //_audioSource.Play();
        return;
    }

    public void StopMusic()
    {
        //_audioSource.Stop();
        return;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (Array.IndexOf(startScenes, sceneName) > -1) {
            if (musicState == MusicState.Start) {
                return;
            }
            musicSource.clip = startBackground;
            musicSource.Play(); 
            musicState = MusicState.Start;
        } else {
            if (musicState == MusicState.Game) {
                return;
            }
            musicSource.clip = gameBackground;
            musicSource.Play();
            musicState = MusicState.Game;
        }
    }
    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
   }
}
