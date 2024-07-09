using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/**
 * InventoryManager is a script that manages the inventory UI in the game.
 * It updates the item counts and coin UI in real-time.
 */
public class InventoryManager : MonoBehaviour
{
    // Reference to UI elements (assuming you have multiple item number texts)
    /**
     * An array of TMP_Text components that display the item counts.
     */
    public TMP_Text[] itemNumbers;
    /**
     * An array of Button components that represent shop items.
     */
    public Button[] shopItemButtons; 
    /**
     * A TMP_Text component that displays the coin count.
     */
    public TMP_Text coins_UI; 

    // Data for UI
    /**
     * An array to store item counts.
     */
    private int[] itemCounts; 
    /**
     * The total number of coins the player has.
     */
    private static int coins; 
    /**
     * A list of TMP_Text components that display the coin count.
     */
    private static List<TMP_Text> coinsUIList = new List<TMP_Text>();

    /**
     * Initializes the inventory manager.
     */
    void Start()
    {
        itemCounts = new int[itemNumbers.Length]; // Initialize item counts array

        // Initialize item counts list with zeros
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            itemNumbers[i].text = "0"; // Initialize text to show 0 items
        }
        UpdateItemNumbers();
        AddCoinUI(coins_UI);
    }

    /**
     * Updates the inventory UI every frame.
     */
    private void Update()
    {
        UpdateItemNumbers();
        UpdateCoinUI();
    }

    /**
     * Updates the UI for all item numbers.
     */
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

    /**
     * Adds a TMP_Text component to the list of coin UI elements.
     * 
     * @param coinText The TMP_Text component to add.
     * @example InventoryManager.AddCoinUI(myCoinText);
     */
    public static void AddCoinUI(TMP_Text coinText){
        if (!coinsUIList.Contains(coinText)){
            coinsUIList.Add(coinText);
        }
        UpdateCoinUI();
    }

    /**
     * Updates the coin UI for all elements in the list.
     */
    public static void UpdateCoinUI(){
        foreach(var coinUI in coinsUIList){
            coinUI.text = ": " + Inventory.GetCoins().ToString(); 
        }
    }
}