using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
     [SerializeField] AudioSource musicSource; 
    //  [SerializeField] AudioSource SFXSource; 
      public AudioClip background; 
   
    void Start()
    {
        musicSource.clip = background; 
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
