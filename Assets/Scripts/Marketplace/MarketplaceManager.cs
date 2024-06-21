using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketplaceManager : MonoBehaviour
{
    public ShopItem[] shopItems; 
    public GameObject[] shopPanelsGameObject; 
    public ShopTemplate[] shopPanels; 
    public Button[] purchaseButton;
    public TMP_Text coins_UI;  // UI element to display coins in the marketplace

    void Start()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanelsGameObject[i].SetActive(true);
        }
        Inventory.AddCoinUI(coins_UI); // Register the marketplace UI element with the Inventory
        Inventory.UpdateCoinUI();
        LoadPanels();
        CheckPurchaseable();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanels[i].titleTxt.text = shopItems[i].title; 
            shopPanels[i].descriptionTxt.text = shopItems[i].description; 
            shopPanels[i].costTxt.text = shopItems[i].baseCost.ToString(); 
        }
    }

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

    public void PurchaseItem(int num)
    {
        if (Inventory.GetCoins() >= shopItems[num].baseCost)
        {
            Inventory.SpendCoins(shopItems[num].baseCost);
            CheckPurchaseable();
            Inventory.AddItemToInventory(num); // Pass item index to add to inventory
        }
    }
}
