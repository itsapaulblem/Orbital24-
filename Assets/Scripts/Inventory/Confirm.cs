using UnityEngine;
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

    void Start()
    {
        if (confirmMenu != null)
        {
            confirmMenu.SetActive(false); // Ensure confirm menu starts deactivated
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
