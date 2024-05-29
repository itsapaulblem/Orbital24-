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

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
          //  Debug.Log("Text file loaded successfully");
        }
        else
        {
           // Debug.LogError("Text file is null!");
        }

        if (endAtLine == 0 && textLines != null)
        {
            endAtLine = textLines.Length - 1;
           //  Debug.Log("End line set to " + endAtLine);
        }
    }

    void Update()
    {
        if (currentLine <= endAtLine)
        {
            theText.text = textLines[currentLine];
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentLine += 1;
            textBox.SetActive(true);
        }

        if (currentLine > endAtLine)
        {
            textBox.SetActive(false);
        }
    }
}
