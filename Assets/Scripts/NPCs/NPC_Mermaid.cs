using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Added for scene management
using UnityEngine.UI;

public class NPC_Mermaid : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText; // Updated for TextMeshPro
    public string[] dialogue;
    private int index;
    public float wordSpeed;

    private Coroutine typingCoroutine;
    public GameObject continueButton;
    public GameObject SpeechBubble;

    private bool dialogueCompleted = false; // Flag to check if dialogue is completed

    public void Start()
    {
        SpeechBubble.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
           // Debug.Log("E key pressed and player is close");
            if (!dialoguePanel.activeInHierarchy && !dialogueCompleted) // Show dialogue panel only if it's not already active and dialogue isn't completed
            {
                dialoguePanel.SetActive(true);
                StartTyping();
            }
            else if (dialoguePanel.activeInHierarchy && !dialogueCompleted) // If dialogue panel is already active and dialogue isn't completed, proceed to next line
            {
                NextLine();
            }
            else if (dialogueCompleted){
                dialoguePanel.SetActive(false);
            }
        }

        if (dialogueText.text == dialogue[index] && continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(Typing());
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = ""; // Clear text before typing starts
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        // Allow the player to proceed to the next line after typing is done
        typingCoroutine = null;
    }

    public void NextLine()
    {
        continueButton.SetActive(false);
        if (index < dialogue.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            zeroText();
            activateInstructions();
            dialogueCompleted = true; // Set flag to true when dialogue is completed
        }
    }

    public void activateInstructions()
    {
        if (SpeechBubble != null)
        {
            //Debug.Log("Activating speech bubble");
            SpeechBubble.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger area");
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger area");
          
            zeroText();
        }
    }
}
