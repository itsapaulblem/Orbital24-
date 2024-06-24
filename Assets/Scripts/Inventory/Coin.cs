using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private AudioManager audioManager;
    void Start(){
        audioManager = AudioManager.Instance;
    }
    // value of one coin
    public int value = 1;

    public void SetValue(int val) {
        value = val;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        if (target != null) {
            Inventory.AddCoins(value);
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.coinCollection); // Play hit sound effect
            Destroy(gameObject);
        }
    }
    private void OnDestroy(){
        InventoryManager.UpdateCoinUI();
    }
}
