using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * MarketplaceManager is a script that manages the marketplace UI and logic.
 * It is responsible for displaying shop items, handling purchases, and updating the coin UI.
 */
public class MarketplaceManager : MonoBehaviour
{
    private AudioManager audioManager;
    /**
     * An array of shop items to be displayed in the marketplace.
     */
    public ShopItem[] shopItems; 
    /**
     * An array of game objects that represent the shop panels.
     */
    public GameObject[] shopPanelsGameObject; 
    /**
     * An array of shop templates that contain UI elements for each shop item.
     */
    public ShopTemplate[] shopPanels; 
    /**
     * An array of purchase buttons that allow the player to buy items.
     */
    public Button[] purchaseButton;
    /**
     * A UI element that displays the player's current coin count.
     */
    public TMP_Text coins_UI;  // UI element to display coins in the marketplace

    /**
     * Initializes the marketplace manager.
     */
    void Start()
    {
        audioManager = AudioManager.Instance;
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanelsGameObject[i].SetActive(true);
        }
        InventoryManager.AddCoinUI(coins_UI); // Register the marketplace UI element with the Inventory
        InventoryManager.UpdateCoinUI();
        LoadPanels();
        CheckPurchaseable();
    }

    /**
     * Loads the shop panels with item data.
     */
    public void LoadPanels()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanels[i].titleTxt.text = shopItems[i].title; 
            shopPanels[i].descriptionTxt.text = shopItems[i].description; 
            shopPanels[i].costTxt.text = shopItems[i].baseCost.ToString(); 
        }
    }

    /**
     * Checks which items are purchaseable based on the player's current coin count.
     */
    public void CheckPurchaseable()
    {
        int coins = Inventory.GetCoins();
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (coins >= shopItems[i].baseCost)
            {
                purchaseButton[i].interactable = true;
            } 
            else if (coins < shopItems[i].baseCost)
            {
                purchaseButton[i].interactable = false;
            }
        }
    }

    /**
     * Purchases an item from the shop.
     * 
     * @param num The index of the item to purchase.
     * @example PurchaseItem(0) would purchase the first item in the shop.
     */
    public void PurchaseItem(int num)
    {
        if (Inventory.GetCoins() >= shopItems[num].baseCost)
        {
            Inventory.SpendCoins(shopItems[num].baseCost);
            InventoryManager.UpdateCoinUI();
            CheckPurchaseable();
            Inventory.AddItemToInventory(num); // Pass item index to add to inventory
            audioManager.PlaySFX(audioManager.purchaseClick);
        }
    }
}