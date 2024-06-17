using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager class handles the Menus and windows of the game, 
/// pauseMenu, gameOverMenu, inventory and miniMapWindow
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance of the GameManager

    // Pause Menu
    private GameObject pauseMenu;
    private bool isPaused = false; // Tracks if the game is paused

    // Gameover Menu 
    private GameObject gameOverMenu;

    // Minimap Window
    private GameObject miniMapWindow;
    private bool isMiniMapActive = false; // Tracks if the minimap is active

    // KillText Window 
    public Text killText;
    private int kills = 0; // Tracks the number of kills

    // Inventory Menu
    private GameObject inventoryMenu; 
    private bool isActive = false; // Tracks if the inventory is active 

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
            Destroy(gameObject); // Destroy duplicate GameManager instances
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        inventoryMenu = GameObject.Find("InventoryMenu"); 
        if (inventoryMenu == null){
            Debug.LogWarning("Inventory Menu not found in the scene: " + scene.name);
        }
        else{
            inventoryMenu.SetActive(false); // Ensure Inventory Menu is inactive 
        }


        miniMapWindow = GameObject.Find("MinimapWindow");
        if (miniMapWindow == null)
        {
            Debug.LogWarning("MinimapWindow not found in the scene: " + scene.name);
        }
        else
        {
            miniMapWindow.SetActive(false); // Ensure MinimapWindow is inactive
        }

        killText = GameObject.Find("KillText")?.GetComponent<Text>();
        if (killText == null)
        {
            Debug.LogWarning("KillText not found in the scene: " + scene.name);
        }

        // Reset kill count when a new scene is loaded
        ResetKillCount();
    }

    private void Update()
    {
        // Check for input to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }

        // Check for input to toggle the minimap
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMiniMap();
        }

        if (Input.GetKeyDown(KeyCode.I)){
            ToggleInventoryMenu();
        }

        
    }

    public void AddKill()
    {
        // Increment the kill count and update the kill text
        kills++;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
       // Update the kill text UI element if the inventory menu is not active
        if (killText != null && !isActive)
        {
            killText.text = kills.ToString() + " KILLS";
        }
        else if (killText != null && isActive)
        {
            killText.text = ""; // Clear the kill text when inventory is active
        }
    }

    private void TogglePauseMenu()
    {
        // Toggle the pause menu's visibility and the game's pause state
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

    private void ToggleInventoryMenu(){
        if (inventoryMenu != null){
            isActive = !isActive; 
            inventoryMenu.SetActive(isActive);
            inventoryMenu.GetComponent<InventoryManager>().UpdateItemNumbers();
            UpdateKillText(); 
        }
        else{
            Debug.LogWarning("InventoryMenu is missing");
        }
    }

    private void ToggleMiniMap()
    {
        // Toggle the minimap's visibility
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
        // Resume the game from the pause state
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Quit()
    {
        // Quit the game and load the start scene
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShowUserDataUI", 1);
        SceneManager.LoadScene("Start");
    }

    public void GameOver()
    {
        // Show the game over menu and pause the game
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
        // Restart the current scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false); // Ensure GameOverMenu is inactive
        }

        ResetKillCount(); // Reset kill count when the game restarts
    }

    public void No()
    {
        // Quit to the start scene
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShowUserDataUI", 1);
        SceneManager.LoadScene("Start");

        ResetKillCount(); // Reset kill count when the game restarts
    }

    private void ResetKillCount()
    {
        // Reset the kill count and update the kill text
        kills = 0;
        UpdateKillText();
    }
}
