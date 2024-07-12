using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedCoin : Coin
{
    // Reference to the AudioManager for playing sounds effects 
    private AudioManager audioManager;
    
    void Start(){
        // Only collectable once
        if (!PlayerPrefsManager.CheckItem(gameObject.name)) { Destroy(gameObject); }
        audioManager = AudioManager.Instance;
    }
}
