using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
public class MarketplaceManager : MonoBehaviour
{

    public int coins; 
    public TMP_Text coins_UI;
    public ShopItem[] shopItems; 
    public GameObject[] shopPanelsGameObject; 
    public ShopTemplate[] shopPanels; 
    public Button[] purchaseButton;
    
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shopItems.Length; i ++){
            shopPanelsGameObject[i].SetActive(true);
        }
        coins_UI.text = "Coins: " + coins.ToString(); 
        LoadPanels();
        CheckPurchaseable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCoins(){
        coins ++;
        coins_UI.text = "Coins: " + coins.ToString();
        CheckPurchaseable();

    }

    public void LoadPanels(){
        for (int i = 0; i < shopItems.Length ; i++){
            shopPanels[i].titleTxt.text = shopItems[i].title; 
            shopPanels[i].descriptionTxt.text = shopItems[i].description; 
            shopPanels[i].costTxt.text = shopItems[i].baseCost.ToString(); 
          
        }
    }

    public void CheckPurchaseable(){
        for (int i = 0; i < shopItems.Length; i ++){
            if (coins >= shopItems[i].baseCost){
                purchaseButton[i].interactable = true;
            } else{
                purchaseButton[i].interactable = false;
            }
        }
    }

    public void PurchaseItem(int num){
        if (coins >= shopItems[num].baseCost){
            coins = coins - shopItems[num].baseCost;
            coins_UI.text = "Coins: " + coins.ToString();
            CheckPurchaseable();
        }
    }
    
}
