using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
 
public class ActivateTextAtLine : MonoBehaviour 
{ 
    public TextAsset theText; 
    public int startLine; 
    public int endLine; 
    public TextBoxManager theTextBox; 
    public bool requireButtonPress; 
    private bool waitForPress; 
    public bool destroyWhenActivated; 
 
    // Start is called before the first frame update 
    void Start() 
    { 
        theTextBox = FindObjectOfType<TextBoxManager>(); 
    } 
 
    // Update is called once per frame 
    void Update() 
    { 
        if (waitForPress && Input.GetKeyDown(KeyCode.E)) 
        { 
            ActivateTextBox(); 
        } 
    } 
 
    void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        { 
            if (requireButtonPress) 
            { 
                waitForPress = true; 
                return; 
            } 
            ActivateTextBox(); 
        } 
    } 
 
    void OnTriggerExit2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        { 
            waitForPress = false; 
        } 
    } 
 
    void ActivateTextBox() 
    { 
        theTextBox.ReloadScript(theText); 
        theTextBox.currentLine = startLine; 
        theTextBox.endAtLine = endLine; 
        theTextBox.EnableTextBox(); 
 
        if (destroyWhenActivated) 
        { 
            Destroy(gameObject); 
        } 
    } 
}