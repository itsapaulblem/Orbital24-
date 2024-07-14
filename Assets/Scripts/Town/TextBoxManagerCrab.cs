using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/**
 * Manages a text box that displays a sequence of lines from a text file.
 * The text box can be enabled or disabled, and the player's movement can be stopped while it is active.
 */
public class TextBoxManagerCrab : MonoBehaviour
{
    /**
     * The game object that represents the text box.
     */
    public GameObject textBox;

    /**
     * The text component that displays the text.
     */
    public Text theText;

    /**
     * The text asset that contains the text lines.
     */
    public TextAsset textFile;

    /**
     * The array of text lines from the text file.
     */
    public string[] textLines;

    /**
     * The current line being displayed.
     */
    public int currentLine;

    /**
     * The last line to display before disabling the text box.
     */
    public int endAtLine;

    /**
     * The player controller that can be stopped while the text box is active.
     */
    public PlayerController player;

    /**
     * Whether the text box is currently active.
     */
    public bool isActive;

    /**
     * Whether to stop the player's movement while the text box is active.
     */
    public bool stopPlayerMovement;

    private bool isTyping = false; 
    private bool cancelTyping = false; 
    public float typeSpeed;

    /**
     * Initializes the text box manager.
     */
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

    /**
     * Updates the text box manager every frame.
     */
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

    /**
     * Scrolls the text line by line, with a delay between each character.
     * 
     * @param lineOfText The text line to scroll.
     */
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

    /**
     * Enables the text box and starts displaying the text lines.
     */
    public void EnableTextBox()
    {
        if (textLines == null || textLines.Length == 0)
        {
            return;
        }
        textBox.SetActive(true);
        isActive = true;

        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    /**
     * Disables the text box and stops displaying the text lines.
     */
    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
    }

    /**
     * Reloads the text lines from a new text asset.
     * 
     * @param newText The new text asset to load.
     */
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