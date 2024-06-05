using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource; 
    public AudioClip enemybeingshot; 
    public AudioClip bobbeingshot; 
    public AudioClip bobshooting; 
    
    // Start is called before the first frame update
    private AudioSource _audioSource; 
    private string prevScene;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
        prevScene = SceneManager.GetActiveScene().name;
    }
    void Start()
    {
        _audioSource.Play();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(prevScene);
        if (prevScene == "Start" && scene.name == "Cutscene1") {
            Destroy(gameObject);
        }
        
    }
    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
   }
}
