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
    public AudioClip dungeonBackground;
    public AudioClip enemybeingshot; 
    public AudioClip bobbeingshot; 
    public AudioClip bobdied; 
    public AudioClip bobshooting;
    public AudioClip coinCollection;  
    public AudioClip confirmClick; 
    public AudioClip bossOne; 
    public AudioClip bossTwo; 
    public AudioClip bossThree; 
    public AudioClip bossField;
    public AudioClip bossFinal; 
    public AudioClip purchaseClick;
    public AudioClip enemyDied;
    public AudioClip munch;

    public static AudioManager Instance;
    private enum MusicState { Start, Game, Dungeon }
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
        // check current scene to determine track to play
        string sceneName = SceneManager.GetActiveScene().name;
        if (Array.IndexOf(startScenes, sceneName) > -1) {
            musicState = MusicState.Start;
            musicSource.clip = startBackground;
            musicSource.Play(); 
        } else if (sceneName == "Dungeon") {
            musicSource.clip = dungeonBackground;
            musicSource.Play();
            musicState = MusicState.Dungeon;
        } else {
            musicState = MusicState.Game;
            musicSource.clip = gameBackground;
            musicSource.Play();
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // on scene change, check current scene and change track if needed
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        if (Array.IndexOf(startScenes, sceneName) > -1) {
            if (musicState == MusicState.Start) {
                return;
            }
            musicSource.clip = startBackground;
            musicSource.Play(); 
            musicState = MusicState.Start;
        } else if (sceneName == "Dungeon") {
            musicSource.clip = dungeonBackground;
            musicSource.Play();
            musicState = MusicState.Dungeon;
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
