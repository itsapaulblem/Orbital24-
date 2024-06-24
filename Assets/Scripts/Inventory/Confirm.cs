using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
    public GameObject confirmMenu;
    public string statToIncrease;  // The stat to increase, e.g., "moveSpeed"
    public float increaseAmount;   // The amount to increase the stat by
    public float increaseDuration = 5f;  // Duration for which the stat increase will be active

    public GameObject inventoryMenu;  // Reference to the inventory menu

    private bool isIncreaseActive = false;
    private float increaseEndTime;
    private AudioManager audioManager; 

    void Start()
    {
        if (confirmMenu != null)
        {
            confirmMenu.SetActive(false); // Ensure confirm menu starts deactivated
        }
        audioManager = AudioManager.Instance; 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
       // confirmMenu = GameObject.Find("ConfirmMenu");
        if (confirmMenu == null)
        {
            Debug.LogWarning("PauseMenu not found in the scene: " + scene.name);
        }
        else
        {
            confirmMenu.SetActive(false); // Ensure PauseMenu is inactive
        }
    }

    void Update()
    {
        if (isIncreaseActive && Time.time >= increaseEndTime)
        {
            // End of temporary increase duration
            isIncreaseActive = false;
            Debug.Log($"{statToIncrease} increase ended after {increaseDuration} seconds.");
        }
    }

    public void OnConfirm()
    {
        if (StatsManager.ofPlayer() != null)
        {
            StatsManager.ofPlayer().IncreaseStat(statToIncrease, increaseAmount);
            Debug.Log($"{statToIncrease} increased by {increaseAmount}");

            // Start temporary increase
            isIncreaseActive = true;
            increaseEndTime = Time.time + increaseDuration;
            audioManager.PlaySFX(audioManager.confirmClick);
        }
        confirmMenu.SetActive(false); // Close the confirm menu after confirming

        // Deactivate inventory menu when confirming
        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(false);
        }
    }

    public void OnCancel()
    {
        confirmMenu.SetActive(false); // Close the confirm menu if cancelled

        // Reactivate inventory menu when cancelling
        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(true);
        }
    }
}
