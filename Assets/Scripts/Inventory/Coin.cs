using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Reference to the AudioManager for playing sounds effects 
    private AudioManager audioManager;
    // Initialize the audioManager instance at the start 
    void Start(){
        audioManager = AudioManager.Instance;
    }
    // Value of one coin, default is 1
    public int value = 1;

    // Method to set the value of the coin 
    public void SetValue(int val) {
        value = val;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        // If the collider is a player, proceed to collect the coin 
        if (target != null) {
            // Add the coin's value to the player's inventory 
            Inventory.AddCoins(value);
            // Ensure audioManager is not null, reassign if necessary 
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            // Play the coin collection sound effect 
            audioManager.PlaySFX(audioManager.coinCollection); 
            Destroy(gameObject);
        }
    }
    private void OnDestroy(){
        // Update the coin UI in the inventory manager 
        InventoryManager.UpdateCoinUI();
    }
}


