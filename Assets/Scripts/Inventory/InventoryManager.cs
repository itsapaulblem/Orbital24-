using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private int[] itemCounts; // Array to store item counts

    // Reference to UI elements (assuming you have multiple item number texts)
    public TMP_Text[] itemNumbers;
    public Button[] shopItemButtons; 
    public TMP_Text coins_UI; 

    void Start()
    {
        itemCounts = new int[itemNumbers.Length]; // Initialize item counts array

        // Initialize item counts list with zeros
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            itemNumbers[i].text = "0"; // Initialize text to show 0 items
        }
        UpdateItemNumbers();
        Inventory.AddCoinUI(coins_UI); // Register the inventory UI element with the Inventory
        Inventory.UpdateCoinUI();
    }

    // Update UI for all item numbers
    public void UpdateItemNumbers()
    {
        itemCounts = Inventory.GetItemCount();
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            itemNumbers[i].text = itemCounts[i].ToString();
            // Ensure button interactability matches item count
            shopItemButtons[i].interactable = itemCounts[i] > 0;
        }
    }
}
