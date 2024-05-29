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

    public bool isActive;
    public bool stopPlayerMovement;

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
            theText.text = textLines[currentLine];
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            currentLine += 1;
            if (currentLine <= endAtLine)
            {
                theText.text = textLines[currentLine];
                Debug.Log("Current Line: " + currentLine);
            }
            else
            {
                DisableTextBox();
            }
        }
    }

    public void EnableTextBox()
    {
        textBox.SetActive(true);
        isActive = true;
        
        player.canMove = false;
        
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
       
        player.canMove = true;
    
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
