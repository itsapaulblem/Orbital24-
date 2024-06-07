using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject pauseMenu; 
    public GameObject GameOverMenu; 
    private bool isPaused = false;
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
    public void Update(){
        if (Input.GetKeyDown(KeyCode.P)){
            TogglePauseMenu(); 
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

    private void TogglePauseMenu(){
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        // Pause or unpause the game
        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }
    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

     public void Quit()
    {
        Time.timeScale = 1f;  
        PlayerPrefs.SetInt("ShowUserDataUI",1);
        SceneManager.LoadScene("Start");
    }

    public void GameOver(){
        GameOverMenu.SetActive(true);
        Time.timeScale = 0f; 
    }

     public void Yes(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void No(){
        Time.timeScale = 1f; 
        PlayerPrefs.SetInt("ShowUserDataUI",1);
        SceneManager.LoadScene("Start");
    }

}
    

