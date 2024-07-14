using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestItem : MonoBehaviour
{
    // Reference to the AudioManager for playing sounds effects 
    private AudioManager audioManager;
    // Initialize the audioManager instance at the start 
    void Start(){
        // Uncomment for repeated testing
        PlayerPrefs.SetInt(gameObject.name, 0);
        
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

            if (Inventory.AllCollected()) {
                StartCoroutine(TransitionToQuest());
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator TransitionToQuest() {
        GameObject fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
        SpriteRenderer fsr = fade.GetComponent<SpriteRenderer>();
        float rate = 1f/ 2f;
        float progress = 0.0f; 
        Color tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fade.SetActive(true);

        while (progress < 1.0f){
            tmp.a = Mathf.Lerp(0, 1 , progress);
            fsr.color = tmp;
            progress += rate * Time.deltaTime;
            yield return null; 
        }

        GameManager.Instance.lastScene = "Town";
        PlayerPrefsManager.SetLastScene("Town");
        PlayerController.SetCoords(5, 2.5f);
        SceneManager.LoadSceneAsync("Town");

        IEnumerator AnimateMovement() {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<PlayerController>().canMove = false;
            player.GetComponent<SpriteRenderer>().flipX = false;
            float rate = 1f/ 2f;
            float progress = 0.0f; 

            while (progress < 1.0f) {
                if (player == null) player = GameObject.Find("Player");
                player.transform.position = new Vector2(Mathf.Lerp(5, 10 , progress), 2.5f);
                progress += rate * Time.deltaTime;
                yield return null; 
            }
            player.GetComponent<PlayerController>().canMove = true;
        }
        CoroutineManager.Instance.StartCoroutine(AnimateMovement());
    }
}
