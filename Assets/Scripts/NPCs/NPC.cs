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

    // Reference to Marketplace menu
    private GameObject marketplaceMenu;

    // Reference to GameManager
    public GameManager gameManager;

    void Start()
    {
        // get reference to the GameManager
        gameManager = GameManager.Instance;

        // get path to dialogue textfile
        currName = gameObject.name;
        path = path + currName + "_dialogue";

        TextAsset dialogueData = Resources.Load<TextAsset>(path);
        dialogue = Regex.Split(dialogueData.text, "\n|\r|\r\n");

        // retrieve dialogueBlock based on story progress
        dialogueBlock = 1;

        dialogueMain = GameObject.Find("DialogueMain");
        if (dialogueMain == null)
        {
            Debug.Log("Missing DialogueMain");
        }

        // attempt to retrieve MarketplaceMenu (used for crab only)
        if (marketplaceMenu == null) { marketplaceMenu = gameManager.marketplaceMenu; }
        if (marketplaceMenu == null)
        {
            Debug.Log("Missing MarketplaceMenu");
        }
        else
        {
            marketplaceMenu.SetActive(false);
        }

        player = FindObjectOfType<PlayerController>();

        // Hardcoded npc names
        npcNames = new Dictionary<string, string>(){
            { "Goldfish", "Finlay" },
            { "Crab", "Krabtain Kidd" },
            { "Mermaid", "Lorelei" },
            { "Field boss", "Ezekill" }
        };

        // Subscribe to the EnemyDied event
        if (currName == "Mermaid") {
            GameObject.Find("StarterEnemy").GetComponent<EnemyAI>().SetInit(1.5f, 20f, 2f);
            EnemyAI.EnemyDied += EnemyDiedHandler;
            Debug.Log("Subscribed to EnemyDied Event");
        }
        
    }

    void OnDestroy()
    {
        // Unsubscribe from the EnemyDied event
        EnemyAI.EnemyDied -= EnemyDiedHandler;
        Debug.Log("Unsubscribed from EnemyDied Event");
    }

    // Event handler for when an enemy dies, only for mermaid
    private void EnemyDiedHandler()
    {
        dialogueBlock = 2; // changed dialogue block from 1 to 2 when enemy dies 
        dialogueShown = false; // Reset dialogueShown when an enemy dies
        Debug.Log("Enemy died, setting dialogueBlock to 2 and resetting dialogueShown");
        state = DialogueState.Next;
        StartCoroutine(RunText());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.transform.Find("Prompt").gameObject.SetActive(true);
            waitForPress = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.transform.Find("Prompt").gameObject.SetActive(false);
            waitForPress = false;
        }
    }

    void Update()
    {
        // Check if the dialogue block is 2 and display the saved message if it hasn't been shown yet
        //if (dialogueBlock == 2 && !dialogueShown)
        //{
        //    Debug.Log("Update: Detected dialogue block 2 and dialogue not shown. Starting saved message coroutine.");
        //    StartCoroutine(RunTextForSavedMessage());
        //}

        // Handle player input for dialogue interaction
        if (waitForPress && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Find("Prompt").gameObject.SetActive(false);
            // Shows dialogue
            // TODO: track story progress for crab marketplace menu
            if (!dialogueShown && state != DialogueState.Next) {
                state = DialogueState.Next;
                StartCoroutine(RunText());
            } else if (state == DialogueState.Next && !nextDialogue) {
                nextDialogue = true;
            } else if (dialogueShown && state == DialogueState.Idle) {
                if (currName != "Crab") {
                    state = DialogueState.Next;
                    StartCoroutine(RunText());
                } else {
                    if (marketplaceMenu == null) { marketplaceMenu = gameManager.marketplaceMenu; }
                    if (marketplaceMenu.activeSelf)
                    {
                        marketplaceMenu.SetActive(false);
                        // Reactivate killText when marketplace menu is hidden
                        if (gameManager.killText != null)
                        {
                            gameManager.killText.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        ShowMarketplaceMenu();
                    }
                }
            }
        }
    }

    /// Activate dialogue panel
    void SetPanel(bool active)
    {
        GameObject panel = dialogueMain.transform.Find("Panel").gameObject;
        if (panel == null)
        {
            Debug.Log("Missing Panel in DialogueMain");
            return;
        }
        panel.SetActive(active);
        panel.transform.Find("Talker").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/NPCs/" + currName + "_big");
        panel.transform.Find("Talker").gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = npcNames[currName];
    }

    IEnumerator RunText()
    {
        SetPanel(true);
        int index = 0;
        // discard previous dialogueBlocks
        for (int i = 0; i < dialogueBlock && !dialogueShown; i++)
        {
            index = Array.IndexOf(dialogue, "-", index) + 1;
        }

        // display dialogue on camera
        while (dialogue[index] != "-")
        {
            nextDialogue = false;
            yield return TextScroll(dialogue[index]);
            index += 1;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            nextDialogue = false;
        }
        state = DialogueState.Idle;
        SetPanel(false);
        dialogueShown = true; // Mark dialogue as shown after it finishes
    }

    IEnumerator RunTextForSavedMessage()
    {
        SetPanel(true);

        // Locate the "You saved me Bob!" line and start from there
        int index = Array.IndexOf(dialogue, "You saved me Bob!");
        Debug.Log("\"You saved me Bob!\" dialogue line index: " + index);

        if (index != -1)
        {
            yield return TextScroll(dialogue[index]);
            index++;
        }

        // Continue displaying the remaining dialogue
        while (index < dialogue.Length && dialogue[index] != "-")
        {
            yield return TextScroll(dialogue[index]);
            index++;
            // Wait for player to press E for the next dialogue line
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            nextDialogue = false;
        }

        state = DialogueState.Idle;
        SetPanel(false); // Deactivate the panel after the last line
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
        while (!nextDialogue && (letter < lineOfText.Length))
        {
            thisText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(0.01f);
        }
        thisText.text = lineOfText;
        nextDialogue = false;
    }

    void ShowMarketplaceMenu()
    {
        if (marketplaceMenu != null && currName == "Crab")
        {
            marketplaceMenu.SetActive(true);
            InventoryManager.UpdateCoinUI();
            if (gameManager.killText != null)
            {
                gameManager.killText.gameObject.SetActive(false);
            }
        }
    }
}
