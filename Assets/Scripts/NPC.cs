using System.Collections;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText; // Updated for TextMeshPro
    public string[] dialogue;
    private int index;
    public float wordSpeed;
    public bool playerIsClose;

    private Coroutine typingCoroutine;
    public GameObject continueButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || playerIsClose)
        {
            Debug.Log("E key pressed and player is close");
            if (dialoguePanel.activeInHierarchy)
            {
                NextLine();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartTyping();
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
        continueButton.SetActive(false);

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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger area");
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger area");
            playerIsClose = false;
            zeroText();
        }
    }
}
