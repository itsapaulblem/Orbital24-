using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Inventory
{
    // Static variable to keep track of the number of coins 
    private static int coins = PlayerPrefs.GetInt("coin",0); 
    // Array to store the count/state of each item type 
    private static int[] itemCounts = new int[5] { 
        PlayerPrefs.GetInt("item0", 0), 
        PlayerPrefs.GetInt("item1", 0),
        PlayerPrefs.GetInt("item2", 0), 
        PlayerPrefs.GetInt("item3", 0), 
        PlayerPrefs.GetInt("item4", 0)
    };
    private static int[] questItemState = new int[3] {
        PlayerPrefs.GetInt("The Blue Bloodstone", 0),
        PlayerPrefs.GetInt("A Vial of Salty Brine", 0),
        PlayerPrefs.GetInt("A Luminous Pearl", 0)
    } ; // 0 - uncollected, 1 - collected, 2 - redeemed

    // Method to add coins to the inventory 
    public static void AddCoins(int amount) {
        coins += amount;
        PlayerPrefs.SetInt("coin", coins);
        // Update the coin UI after adding coins 
        InventoryManager.UpdateCoinUI();
    }
    // Method to spend coins from the inventory 
    public static void SpendCoins(int amount) {
        coins -= amount;
        PlayerPrefs.SetInt("coin", coins);
    }
    // Method to get the current number of coins 
    public static int GetCoins() {
        return coins;
    }
    
    // Method to add an item to the inventory by its index 
    public static void AddItemToInventory(int itemIndex)
    {
        itemCounts[itemIndex]++;
        PlayerPrefs.SetInt("item"+itemIndex, itemCounts[itemIndex]);
    }
    // Method to consume an item to the inventory by its index 
    public static void ConsumeItemFromInventory(int itemIndex)
    {
        itemCounts[itemIndex]--;
        PlayerPrefs.SetInt("item"+itemIndex, itemCounts[itemIndex]);
    }

    // Method to get the current counts of all items 
    public static int[] GetItemCount()
    {
        return itemCounts;
    }

    private static Dictionary<string,int> QuestIndex = new Dictionary<string, int>(){
        {"The Blue Bloodstone", 0}, {"A Vial of Salty Brine", 1}, {"A Luminous Pearl", 2}
    };
    // Method to add a quest item to the inventory by its name 
    public static void AddQuestItemToInventory(string key) {
        questItemState[QuestIndex[key]] = 1;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.SetInt("Goldfish", 3);
    }
    
    public static string RedeemQuestItem() {
        string output = "";
        int valid = 0;
        foreach (int s in questItemState) {
            if (s == 1) { valid += 1; }
        }
        foreach (KeyValuePair<string, int> idx in QuestIndex) {
            if (questItemState[idx.Value] == 1) {
                if (output != "") { output = output + (valid--==3 ? ", " : " and "); }
                output = output + idx.Key;
                questItemState[idx.Value]++;
                PlayerPrefs.SetInt(idx.Key, 2);
            }
        }
        PlayerPrefsManager.DecrDialogueBlock("Goldfish");
        return output;
    }

    public static bool QuestDone() {
        foreach (int s in questItemState) {
            if (s != 2) return false;
        }
        return true;
    }

    public static bool AllCollected() {
        foreach (int s in questItemState) {
            if (s == 0) return false;
        }
        return true;
    }
}
