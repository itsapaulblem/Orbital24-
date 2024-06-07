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

    void Awake() {
        currName = gameObject.name;
        path = path + currName + "_dialogue";
        
        TextAsset dialogueData = Resources.Load<TextAsset>(path);
        dialogue = Regex.Split ( dialogueData.text, "\n|\r|\r\n");

        // TODO: Firebase access to determine dialogue block
        dialogueBlock = 1;

        dialogueMain = GameObject.Find("DialogueMain");
        if (dialogueMain == null) {
            Debug.Log("Missing DialogueMain");
        }

        player = FindObjectOfType<PlayerController>();

        // Hard coded names for npc
        npcNames = new Dictionary<string, string>(){
                    { "Goldfish", "Finlay" },
                    { "Crab", "Krabtain Kidd" },
                    { "Mermaid", "Lorelei" },
                    { "Field boss", "Ezekill" }};
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) {
            gameObject.transform.Find("Prompt").gameObject.SetActive(true);    // TODO: After tutorial, disable?
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

    // Update is called once per frame
    void Update()
    {
        if (waitForPress && Input.GetKeyDown(KeyCode.E)) {
            gameObject.transform.Find("Prompt").gameObject.SetActive(false);
            if (state != DialogueState.Next){
                StartCoroutine(RunText());
                Debug.Log("Trigger correct");
            } else if (state == DialogueState.Next && !nextDialogue) {
                Debug.Log("Trigger cancel");
                nextDialogue = true;
            }
        }
    }

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
        if (state == DialogueState.Run) {
            state = DialogueState.Next;
            // disable player movement
            
            for (int i = 0; i < dialogueBlock; i++) {
                index = Array.IndexOf(dialogue, "-", index) + 1;
            }
        } else { state = DialogueState.Next; }

        while (dialogue[index] != "-") {
            yield return TextScroll(dialogue[index]);
            index += 1;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            nextDialogue = false;
        }
        state = DialogueState.Idle;
        SetPanel(false);
    }

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
}
