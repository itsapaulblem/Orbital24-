using UnityEngine;

public class Confirm : MonoBehaviour
{
    public GameObject confirmMenu;
    public string statToIncrease;  // The stat to increase, e.g., "moveSpeed"
    public int itemIndex;
    public float increaseAmount;   // The amount to increase the stat by

    public GameObject inventoryMenu;  // Reference to the inventory menu

    private void Start()
    {
        if (confirmMenu != null)
        {
            confirmMenu.SetActive(false); // Ensure confirm menu starts deactivated
        }
    }

    public void OnConfirm()
    {
        if (StatsManager.ofPlayer() != null)
        {
            // Consume item
            Inventory.ConsumeItemFromInventory(itemIndex);

            // Increase the stat using StatsManager for a specific duration
            StatsManager.ofPlayer().TemporaryIncreaseStat(statToIncrease, increaseAmount);
            Debug.Log($"{statToIncrease} increased by {increaseAmount}");

            // Start the temporary increase
            Debug.Log($"Temporary {statToIncrease} increase for (additional) 100 seconds.");

            confirmMenu.SetActive(false); // Close the confirm menu after confirming

            // Deactivate inventory menu when confirming
            if (inventoryMenu != null)
            {
                inventoryMenu.SetActive(false);
            }
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
