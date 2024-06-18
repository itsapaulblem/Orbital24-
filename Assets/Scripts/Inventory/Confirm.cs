using UnityEngine;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
    public GameObject confirmMenu;
    public string statToIncrease;  // The stat to increase, e.g., "moveSpeed"
    public float increaseAmount;   // The amount to increase the stat by

    void Start()
    {
        if (confirmMenu != null)
        {
            confirmMenu.SetActive(false); // Ensure confirm menu starts deactivated
        }
    }

    public void OnConfirm()
    {
        if (StatsManager.ofPlayer() != null)
        {
            StatsManager.ofPlayer().IncreaseStat(statToIncrease, increaseAmount);
            Debug.Log($"{statToIncrease} increased by {increaseAmount}");
        }
        confirmMenu.SetActive(false); // Close the confirm menu after confirming
    }

    public void OnCancel()
    {
        confirmMenu.SetActive(false); // Close the confirm menu if cancelled
    }
}
