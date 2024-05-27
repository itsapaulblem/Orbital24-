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
    public float wordSpeed = 0.1f; // Ensure a default value for wordSpeed

    private Coroutine typingCoroutine;
    public GameObject continueButton;
    public GameObject SpeechBubble;

    private void Start()
    {
        if (SpeechBubble != null)
        {
            SpeechBubble.SetActive(false);
        }
        else
        {
            Debug.LogError("SpeechBubble is not assigned!");
        }

        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("DialoguePanel or DialogueText is not assigned!");
        }

        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialoguePanel != null && !dialoguePanel.activeInHierarchy)
            {
                dialoguePanel.SetActive(true);
                StartTyping();
            }
            else if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
            {
                NextLine();
            }
        }

        if (dialogueText != null && dialogueText.text == dialogue[index] && continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    private void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(Typing());
    }

    public void ZeroText()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        index = 0;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

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

    private IEnumerator Typing()
    {
        if (dialogueText != null)
        {
            dialogueText.text = ""; // Clear text before typing starts
            foreach (char letter in dialogue[index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
            typingCoroutine = null; // Reset the coroutine reference
        }
    }

    public void NextLine()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (index < dialogue.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            ZeroText();
            ActivateInstructions();
        }
    }

    public void ActivateInstructions()
    {
        if (SpeechBubble != null)
        {
            SpeechBubble.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ZeroText();
        }
    }

    public void ActivateDialoguePanel()
    {
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            StartTyping();
        }
    }
}
