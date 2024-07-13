using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedCoin : Coin
{
    void Start(){
        // Only collectable once
        if (!PlayerPrefsManager.CheckItem(gameObject.name)) { Destroy(gameObject); }
        audioManager = AudioManager.Instance;
    }
}
