using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    // Dialogue Identity
    private string currName;
    private string path = "Dialogue/";
    private string[] dialogue;
    private int dialogueBlock = 1;

    // Dialogue Display
    private GameObject dialogueMain;
    private bool nextDialogue = false;
    private enum DialogueState { Run, Next, Idle }
    private DialogueState state = DialogueState.Run;
    private Dictionary<string, string> npcNames;

    // Player Inputs
    private bool waitForPress = false;
    private PlayerController player;

    // Track if dialogue has been shown
    private bool dialogueShown = false;

    // Marketplace menu
    private GameObject marketplaceMenu;

    //reference to GameManager
    private GameManager gameManager; 

    void Awake() {
        // get path to dialogue textfile
        currName = gameObject.name;
        path = path + currName + "_dialogue";
        
        TextAsset dialogueData = Resources.Load<TextAsset>(path);
        dialogue = Regex.Split(dialogueData.text, "\n|\r|\r\n");

        // TODO: retrieve dialogueBlock based on story progress
        dialogueBlock = 1;

        dialogueMain = GameObject.Find("DialogueMain");
        if (dialogueMain == null) {
            Debug.Log("Missing DialogueMain");
        }

        // attempt to retrieve MarketplaceMenu (used for crab only)
        marketplaceMenu = GameObject.Find("MarketplaceMenu");
        if (marketplaceMenu == null) {
            Debug.Log("Missing MarketplaceMenu");
        } else {
            marketplaceMenu.SetActive(false);
        }

        player = FindObjectOfType<PlayerController>();
        // get reference to the GameManager
        gameManager = GameManager.Instance; 

        // Hardcoded npc names
        npcNames = new Dictionary<string, string>(){
            { "Goldfish", "Finlay" },
            { "Crab", "Krabtain Kidd" },
            { "Mermaid", "Lorelei" },
            { "Field boss", "Ezekill" }
        };
    }

    void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) {
            gameObject.transform.Find("Prompt").gameObject.SetActive(true);
            waitForPress = true;
        } 
    } 
 
    void OnTriggerExit2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) { 
            gameObject.transform.Find("Prompt").gameObject.SetActive(false);
            waitForPress = false; 
            Debug.Log("Waiting false");
        } 
    } 

    void Update()
    {
        if (waitForPress && Input.GetKeyDown(KeyCode.E)) {
            gameObject.transform.Find("Prompt").gameObject.SetActive(false);
            // shows dialogue
            // TODO: track story progress for crab marketplace menu
            if (!dialogueShown) {
                if (state != DialogueState.Next){
                    state = DialogueState.Next;
                    StartCoroutine(RunText());
                } else if (state == DialogueState.Next && !nextDialogue) {
                    nextDialogue = true;
                }
            } else {
               if (marketplaceMenu.activeSelf){
                marketplaceMenu.SetActive(false);
                // Reacticate killText when marketplace menu is hidden 
                if (gameManager.killText != null){
                    gameManager.killText.gameObject.SetActive(true);
                }
               }
               else{
                ShowMarketplaceMenu();
               }
            }
        }
    }

    /// Activate dialogue panel
    void SetPanel(bool active) {
        GameObject panel = dialogueMain.transform.Find("Panel").gameObject;
        if (panel == null) {
            Debug.Log("Missing Panel in DialogueMain");
            return;
        }
        panel.SetActive(active);
        panel.transform.Find("Talker").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/NPCs/" + currName + "_big");
        panel.transform.Find("Talker").gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = npcNames[currName];
    }

    IEnumerator RunText() {
        SetPanel(true);
        int index = 0;
        // discard previous dialogueBlocks
        for (int i = 0; i < dialogueBlock; i++) {
            index = Array.IndexOf(dialogue, "-", index) + 1;
        }

        // display dialogue on camera
        while (dialogue[index] != "-") {
            yield return TextScroll(dialogue[index]);
            index += 1;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            nextDialogue = false;
        }
        state = DialogueState.Idle;
        SetPanel(false);
        dialogueShown = true; // Mark dialogue as shown after it finishes
    }

    // Handles text display animations
    private IEnumerator TextScroll(string lineOfText)
    {
        int letter = 0;
        TextMeshProUGUI thisText = dialogueMain.transform.Find("Panel").gameObject
                                           .transform.Find("Dialogue").gameObject
                                           .GetComponent<TextMeshProUGUI>();
        
        thisText.text = "";
        nextDialogue = false;
        while (!nextDialogue && (letter < lineOfText.Length))
        {
            thisText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(0.01f);
        }
        thisText.text = lineOfText;
        nextDialogue = false;
    }

    void ShowMarketplaceMenu() {
        if (marketplaceMenu != null) {
            marketplaceMenu.SetActive(true);
            if (gameManager.killText != null){
                gameManager.killText.gameObject.SetActive(false);
            }
        } 
}
}
