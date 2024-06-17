using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    // Array to store item counts
    private static int[] itemCounts = new int[5];

    public static void AddItemToInventory(int itemIndex)
    {
        itemCounts[itemIndex]++;
    }

    public static int[] GetItemCount()
    {
        return itemCounts;
    }
}
