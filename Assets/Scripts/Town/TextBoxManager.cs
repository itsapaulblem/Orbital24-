using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
    public GameObject prompt;
    public GameObject speechPanel;
    public Text theText;
    public TextAsset textFile;
    public string[] textLines;
    public int currentLine;
    public int endAtLine;
    public PlayerController player;

    public bool isTextboxActive; 
    private bool isTyping = false;
    private bool cancelTyping = false;
    public float typeSpeed;
    public bool isPlayerNear = false; 

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (textFile != null)
        {
            textLines = textFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        }

        if (endAtLine == 0 && textLines != null)
        {
            endAtLine = textLines.Length - 1;
        }
    }

    void Update()
    {
        // Show prompt if textbox is not active and player is near
        if (!isTextboxActive && isPlayerNear)
        {
            prompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)){
                prompt.SetActive(false);
                EnableTextBox();
            }
        }

        // Check for 'E' key press when player is near
        if (isTextboxActive && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTyping)
            {
                currentLine += 1;
                if (currentLine > endAtLine)
                {
                    DisableTextBox();
                }
                else
                {
                    StartCoroutine(TextScroll(textLines[currentLine]));
                }
            }
            else if (isTyping && !cancelTyping)
            {
                cancelTyping = true;
            }
        }
    }

    private IEnumerator TextScroll(string lineOfText)
    {
        int letter = 0;
        theText.text = "";
        isTyping = true;
        cancelTyping = false;
        while (isTyping && !cancelTyping && (letter < lineOfText.Length))
        {
            theText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(typeSpeed);
        }
        theText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }

    public void EnableTextBox()
    {
        if (textLines == null || textLines.Length == 0)
        {
            return;
        }
        speechPanel.SetActive(true);
        prompt.SetActive(false);
        isTextboxActive = true; // Update flag to indicate textbox is active
        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void DisableTextBox()
    {
        speechPanel.SetActive(false);
        isTextboxActive = false; // Update flag to indicate textbox is inactive
    }

    public void ReloadScript(TextAsset newText)
    {
        if (newText != null)
        {
            textLines = newText.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            currentLine = 0;
            endAtLine = textLines.Length - 1;
        }
    }

    public void showPrompt(bool show)
    {
        prompt.SetActive(show);
    }
}