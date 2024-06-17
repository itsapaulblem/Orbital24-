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

    void Start()
    {
        itemCounts = new int[itemNumbers.Length]; // Initialize item counts array

        // Initialize item counts list with zeros
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            itemNumbers[i].text = "0"; // Initialize text to show 0 items
        }
        UpdateItemNumbers();
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
