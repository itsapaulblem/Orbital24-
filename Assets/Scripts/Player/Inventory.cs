using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Inventory
{
    private static int coins; 
    // Array to store item counts
    private static int[] itemCounts = new int[5];

    public static void AddCoins(int amount) {
        coins += amount;
        InventoryManager.UpdateCoinUI();
    }

    public static void SpendCoins(int amount) {
        coins -= amount;
    }

    public static int GetCoins() {
        return coins;
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
