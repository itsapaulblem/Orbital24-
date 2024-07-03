using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase; 
using Firebase.Auth; 
using Firebase.Database;
using Firebase.Extensions; 
using System.Threading.Tasks;



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
    public bool isActive = false; // Tracks if the inventory is active 

    // Marketplace Menu
    public GameObject marketplaceMenu;

    // Sign Out Menu
    public GameObject signoutMenu;
    public bool isSignOutActive = false; // Initialize isSignOutActive to false
    private FirebaseAuth auth; 

    private Vector3 lastPlayerPosition; // tracks the player's last position
    private string lastScene; // tracks the last scene name 

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
        
        Debug.Log("Scene loaded: " + scene.name);

        if (pauseMenu == null) { pauseMenu = GameObject.Find("PauseMenu"); }
        if (pauseMenu == null) {
            Debug.LogWarning("PauseMenu not found in the scene: " + scene.name);
        } else {
            pauseMenu.SetActive(false); // Ensure PauseMenu is inactive
        }

        if (gameOverMenu == null) { gameOverMenu = GameObject.Find("GameOverMenu"); }
        if (gameOverMenu == null) {
            Debug.LogWarning("GameOverMenu not found in the scene: " + scene.name);
        } else {
            gameOverMenu.SetActive(false); // Ensure GameOverMenu is inactive
        }

        if (inventoryMenu == null) { inventoryMenu = GameObject.Find("InventoryMenu"); }
        if (inventoryMenu == null) {
            Debug.LogWarning("InventoryMenu not found in the scene: " + scene.name);
        } else {
            inventoryMenu.SetActive(false); // Ensure InventoryMenu is inactive 
        }

        if (miniMapWindow == null) { miniMapWindow = GameObject.Find("MinimapWindow"); }
        if (miniMapWindow == null) {
            Debug.LogWarning("MinimapWindow not found in the scene: " + scene.name);
        } else {
            miniMapWindow.SetActive(false); // Ensure MinimapWindow is inactive
        }

        if (killText == null) { killText = GameObject.Find("KillText")?.GetComponent<Text>(); }
        if (killText == null) {
            Debug.LogWarning("KillText not found in the scene: " + scene.name);
        }

        if (marketplaceMenu == null) { marketplaceMenu = GameObject.Find("MarketplaceMenu"); }
        if (marketplaceMenu == null) {
            Debug.LogWarning("MarketplaceMenu not found in the scene: " + scene.name);
        } else {
            marketplaceMenu.SetActive(false); // Ensure MarketplaceMenu is inactive
        }

        if (signoutMenu == null) {
            Debug.LogWarning("SignOutMenu not found in the scene: " + scene.name);
        } else {
            signoutMenu.SetActive(false); // Ensure SignOutMenu is inactive
            Debug.Log("SignOutMenu found and set to inactive");
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

    public void ToggleInventoryMenu()
    {
        if (inventoryMenu != null)
        {
            isActive = !isActive; 
            inventoryMenu.SetActive(isActive);
            InventoryManager.UpdateCoinUI();
            inventoryMenu.GetComponent<InventoryManager>().UpdateItemNumbers();
            UpdateKillText(); 
        }
        else
        {
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
    // For the sign out menu to pop up 
    public void SignOutMenu()
    {
        // Quit the game and sign out 
        Time.timeScale = 1f;
        if (signoutMenu != null)
        {
            pauseMenu.SetActive(false);
            signoutMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SignOutMenu is missing.");
        }
    }
    
    // function when player changes their mind and resume playing the game 
    public void Resume(){
        signoutMenu.SetActive(false);
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
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Room");
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.Heal(-1);

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false); // Ensure GameOverMenu is inactive
        }

        ResetKillCount(); // Reset kill count when the game restarts
    }
    // function used when player signs out in pause menu or says no in game over menu 
    public void Quit()
    {
        // Quit to the start scene
        Time.timeScale = 1f;
        SavePlayerProgress();
        auth.SignOut();
        SceneManager.LoadScene("Start");
        ResetKillCount(); // Reset kill count when the game restarts
    }

    private void ResetKillCount()
    {
        // Reset the kill count and update the kill text
        kills = 0;
        UpdateKillText();
    }

    // New method to set the inventory menu active state
    public void SetInventoryMenuActive(bool isActive)
    {
        if (inventoryMenu != null)
        {
            this.isActive = isActive;
            inventoryMenu.SetActive(isActive);
            UpdateKillText();
        }
        else
        {
            Debug.LogWarning("InventoryMenu is missing");
        }
    }
    // Save player progress to Firebase
    public void SavePlayerProgress(){
        PlayerPrefs.SetInt("kills", kills);
        PlayerPrefs.SetString("lastScene", SceneManager.GetActiveScene().name); 
        PlayerPrefs.SetFloat("lastPosX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("lastPosY", lastPlayerPosition.y);
        PlayerPrefs.SetFloat("lastPosZ", lastPlayerPosition.z);
        PlayerPrefs.Save(); // Save PlayerPrefs data immediately
    }
    // Load player progress from Firebase
    public void LoadPlayerProgress()
    {
        // Load player progress including position from PlayerPrefs
        kills = PlayerPrefs.GetInt("kills", 0);
        lastScene = PlayerPrefs.GetString("lastScene", "Cutscene1");
        float posX = PlayerPrefs.GetFloat("lastPosX", 0f);
        float posY = PlayerPrefs.GetFloat("lastPosY", 0f);
        float posZ = PlayerPrefs.GetFloat("lastPosZ", 0f);
        lastPlayerPosition = new Vector3(posX, posY, posZ);

        // Load the last scene the player was in
        SceneManager.LoadScene(lastScene);

        // Move the player to the last saved position
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.transform.position = lastPlayerPosition;
        }

        // Update UI or perform other actions based on loaded data
        UpdateKillText();
    }

   
}

