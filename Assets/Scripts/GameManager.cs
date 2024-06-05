using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text killText;
    private int kills = 0;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        kills++;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        killText.text = kills.ToString() + " KILLS";
    }
}
