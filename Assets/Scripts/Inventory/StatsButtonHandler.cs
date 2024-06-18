using UnityEngine;
using UnityEngine.UI;

public class StatButtonHandler : MonoBehaviour
{
    public string statToIncrease;  // The stat to increase, e.g., "moveSpeed"
    public float increaseAmount;   // The amount to increase the stat by
    public GameObject confirmMenu; // Reference to the confirm menu GameObject
    public GameObject inventoryMenu;  // Reference to the inventory menu

    private Button button;         // Reference to the button component

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        if (confirmMenu != null)
        {
            Confirm confirmScript = confirmMenu.GetComponent<Confirm>();
            if (confirmScript != null)
            {
                confirmScript.statToIncrease = statToIncrease;
                confirmScript.increaseAmount = increaseAmount;
            }

            confirmMenu.SetActive(true); // Open the confirm menu
            inventoryMenu.SetActive(false);
        }
    }
}
