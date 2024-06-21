using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Inventory
{
    private static int coins; 
    private static List<TMP_Text> coinsUIList = new List<TMP_Text>();
    // Array to store item counts
    private static int[] itemCounts = new int[5];

    public static void AddCoinUI(TMP_Text coinText)
    {
        if (!coinsUIList.Contains(coinText))
        {
            coinsUIList.Add(coinText);
            UpdateCoinUI(); // Update immediately to reflect current coin count
        }
    }

    public static void AddCoins()
    {
        coins++;
        UpdateCoinUI();
    }

    public static void SpendCoins(int amount)
    {
        coins -= amount;
        UpdateCoinUI();
    }

    public static int GetCoins()
    {
        return coins;
    }

    public static void UpdateCoinUI()
    {
        foreach (var coinUI in coinsUIList)
        {
            coinUI.text = ": " + coins.ToString();
        }
    }

    public static void AddItemToInventory(int itemIndex)
    {
        itemCounts[itemIndex]++;
    }

    public static int[] GetItemCount()
    {
        return itemCounts;
    }
}
