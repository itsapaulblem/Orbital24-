using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Confirm : MonoBehaviour
{
    public GameObject confirmMenu;
    public string statToIncrease;  // The stat to increase, e.g., "moveSpeed"
    public float increaseAmount;   // The amount to increase the stat by
    private float increaseDuration = 5f;  // Duration for which the stat increase will be active

    public GameObject inventoryMenu;  // Reference to the inventory menu

    private bool isIncreaseActive = false;
    private float increaseEndTime;
    private AudioManager audioManager;
    private Renderer playerRenderer;
    private Color originalColor;
    private Color flashColor = new Color(0.5f, 0f, 0.5f); // Purple color
    private float flashDuration = 0.5f; // Duration for each flash

    void Start()
    {
        if (confirmMenu != null)
        {
            confirmMenu.SetActive(false); // Ensure confirm menu starts deactivated
        }
        audioManager = AudioManager.Instance;

        // Assuming the player has a tag "Player" and has a Renderer component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                originalColor = playerRenderer.material.color;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
           // isIncreaseActive = false;
            Debug.Log($"{statToIncrease} increase ended after {increaseDuration} seconds.");

            // Revert the stat using StatsManager
            StatsManager.ofPlayer().RevertStat(statToIncrease);
            Debug.Log("Stat back to original");

            // Stop the flashing effect and reset color
            if (playerRenderer != null)
            {
                StopCoroutine(FlashColor()); 
                Debug.Log("Normal colour resume");
                playerRenderer.material.color = originalColor;
            }
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

            // Start flashing color effect
            if (playerRenderer != null)
            {
                StartCoroutine(FlashColor());
                Debug.Log("Colour change");
            }
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

    private IEnumerator FlashColor()
    {
        while (isIncreaseActive)
        {
            playerRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
