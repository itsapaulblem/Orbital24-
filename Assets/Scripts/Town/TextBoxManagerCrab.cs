using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManagerCrab : MonoBehaviour
{
    public GameObject textBox;
    public Text theText;
    public TextAsset textFile;
    public string[] textLines;
    public int currentLine;
    public int endAtLine;
    public PlayerController player;

    public bool isActive;
    public bool stopPlayerMovement;

    private bool isTyping = false; 
    private bool cancelTyping = false; 
    public float typeSpeed;

    // Start is called before the first frame update
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

        if (isActive)
        {
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (currentLine <= endAtLine && textLines != null && currentLine < textLines.Length)
        {
            // theText.text = textLines[currentLine];
        }

        if (Input.GetKeyDown(KeyCode.E))
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
        textBox.SetActive(true);
        isActive = true;

        if (player != null)
        {
            player.canMove = false;
        }

        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;

        if (player != null)
        {
            player.canMove = true;
        }
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
}
