using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

        if (isActive){
            EnableTextBox();
        }
        else{
            DisableTextBox();
        }
    }

    void Update()
    {
        if  (!isActive){
            return; 
        }
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
            DisableTextBox();
        }
    }

    public void EnableTextBox(){
         textBox.SetActive(true);
         if (stopPlayerMovement){
            player.canMove = false; 
         }
    }

    public void DisableTextBox(){
        textBox.SetActive(false);
        isActive = false;
        player.canMove = true; 
    }

    public void ReloadScript(TextAsset theText){
        if (theText != null){
            textLines = new string[1]; 
            textLines = theText.text.Split('\n');
        }
    }
}
