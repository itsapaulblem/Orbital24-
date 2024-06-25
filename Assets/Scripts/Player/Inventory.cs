using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Inventory
{
    // Static variable to keep track of the number of coins 
    private static int coins = 50; 
    // Array to store the count of each item type 
    private static int[] itemCounts = new int[5];

    // Method to add coins to the inventory 
    public static void AddCoins(int amount) {
        coins += amount;
        // Update the coin UI after adding coins 
        InventoryManager.UpdateCoinUI();
    }
    // Method to spend coins from the inventory 
    public static void SpendCoins(int amount) {
        coins -= amount;
    }
    // Method to get the current number of coins 
    public static int GetCoins() {
        return coins;
    }
    
    // Method to add an item to the inventory by its index 
    public static void AddItemToInventory(int itemIndex)
    {
        itemCounts[itemIndex]++;
    }
    // Method to consume an item to the inventory by its index 
    public static void ConsumeItemFromInventory(int itemIndex)
    {
        itemCounts[itemIndex]--;
    }

    // Method to get the current counts of all items 
    public static int[] GetItemCount()
    {
        return itemCounts;
    }
}
