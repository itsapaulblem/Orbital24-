using UnityEngine;

public class ActivateTextAtLine : MonoBehaviour
{
    public TextAsset theText;
    public int startLine;
    public int endLine;
    public TextBoxManager theTextBox;

    // Start is called before the first frame update
    void Start()
    {
        theTextBox = FindObjectOfType<TextBoxManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("fdfb");
            theTextBox.showPrompt(true); // Show prompt when player is near
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            theTextBox.showPrompt(false); // Hide prompt when player moves away
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (theTextBox.isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ActivateTextBox(); // Activate text box when player presses 'E' while being near
        }
    }

    void ActivateTextBox()
    {
        theTextBox.ReloadScript(theText);
        theTextBox.currentLine = startLine;
        theTextBox.endAtLine = endLine;
        theTextBox.EnableTextBox();
    }
}
