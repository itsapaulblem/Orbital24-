using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    // Reference to the AudioManager for playing sounds effects 
    private AudioManager audioManager;
    // Initialize the audioManager instance at the start 
    void Start(){
        // Uncomment for repeated testing
        //PlayerPrefs.SetInt(gameObject.name, 0);
        
        if (!PlayerPrefsManager.CheckItem(gameObject.name)) { Destroy(gameObject); }
        audioManager = AudioManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        // If the collider is a player, proceed to collect the item 
        if (target != null) {
            // Add the quest item to the player's inventory 
            Inventory.AddQuestItemToInventory(gameObject.name);
            // Ensure audioManager is not null, reassign if necessary 
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            // Play the coin collection sound effect 
            audioManager.PlaySFX(audioManager.coinCollection); // reusing sfx
            Destroy(gameObject);
        }
    }
}
