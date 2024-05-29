using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
    public GameObject textBox;
    public Text theText;
    public TextAsset textFile;
    public string[] textLines;
    public int currentLine;
    public int endAtLine;
    public PlayerController player;
    private Coroutine typingCoroutine;
    public GameObject continueButton;
    public float wordSpeed;
    public bool playerIsClose; // Renamed from playerClose to match the other code

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }

        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }
    }

    void Update()
    {
        if (playerIsClose || Input.GetKeyDown(KeyCode.E))
        {
            if (!textBox.activeInHierarchy)
            {
                textBox.SetActive(true);
                StartTyping();
            }
            else
            {
                if (theText.text == textLines[currentLine])
                {
                    NextLine();
                }
            }
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

    IEnumerator Typing()
    {
        theText.text = ""; // Clear text before typing starts
        foreach (char letter in textLines[currentLine].ToCharArray())
        {
            theText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
        typingCoroutine = null;
    }

    public void NextLine()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }
        
        if (currentLine < endAtLine)
        {
            currentLine++;
            StartTyping();
        }
        else
        {
            textBox.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            if (textBox.activeInHierarchy)
            {
                textBox.SetActive(false);
            }
        }
    }
}
