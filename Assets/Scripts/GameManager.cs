using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject pauseMenu;
    private GameObject gameOverMenu;
    private GameObject miniMapWindow; 
    private bool isMiniMapActive = false; 
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
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-establish references when a new scene is loaded
        pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu == null)
        {
            Debug.LogWarning("PauseMenu not found in the scene: " + scene.name);
        }
        else
        {
            pauseMenu.SetActive(false); // Ensure PauseMenu is inactive
        }

        gameOverMenu = GameObject.Find("GameOverMenu");
        if (gameOverMenu == null)
        {
            Debug.LogWarning("GameOverMenu not found in the scene: " + scene.name);
        }
        else
        {
            gameOverMenu.SetActive(false); // Ensure GameOverMenu is inactive
        }

        miniMapWindow = GameObject.Find("MinimapWindow");
        if (miniMapWindow == null){
            Debug.LogWarning("MinimapWindow not found in the scene: " + scene.name);
        }
        else{
            miniMapWindow.SetActive(false);
        }


        killText = GameObject.Find("KillText")?.GetComponent<Text>();
        if (killText == null)
        {
            Debug.LogWarning("KillText not found in the scene: " + scene.name);
        }

        ResetKillCount(); // Reset kill count when scene is loaded
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.M)){
            ToggleMiniMap();
        }
    }

    public void AddKill()
    {
        kills++;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        if (killText != null)
        {
            killText.text = kills.ToString() + " KILLS";
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);

            // Pause or unpause the game
            Time.timeScale = isPaused ? 0f : 1f;
        }
        else
        {
            Debug.LogWarning("PauseMenu is missing.");
        }
    }

     private void ToggleMiniMap()
    {
    
        if (miniMapWindow != null)
        {
           isMiniMapActive = !isMiniMapActive;
            miniMapWindow.SetActive(isMiniMapActive);
        }
        else
        {
            Debug.LogWarning("MiniMapWindow is missing.");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShowUserDataUI", 1);
        SceneManager.LoadScene("Start");
    }

    public void GameOver()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverMenu is missing.");
        }

        Time.timeScale = 0f;
    }

    public void Yes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false); // Ensure GameOverMenu is inactive
        }

        ResetKillCount(); // Reset kill count when game restarts
    }

    public void No()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShowUserDataUI", 1);
        SceneManager.LoadScene("Start");

        ResetKillCount(); // Reset kill count when game restarts
    }

    private void ResetKillCount()
    {
        kills = 0;
        UpdateKillText(); // Update kill count text after resetting
    }
}
