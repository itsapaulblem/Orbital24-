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

    public GameObject difficultyMenu; 
    public bool isdifficultyMenuActive = false; 
    public string lastScene; // tracks the last scene name 
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
        IEnumerator FadeIn() {
            GameObject fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
            SpriteRenderer fsr = fade.GetComponent<SpriteRenderer>();
            float rate = 1f/ 2f;
            float progress = 0.0f; 
            Color tmp = fsr.color;
            fade.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            while (progress < 1.0f){
                tmp.a = Mathf.Lerp(1, 0 , progress);
                fsr.color = tmp;
                progress += rate * Time.deltaTime;
                yield return null; 
            }
            fade.SetActive(false);
        }
        StartCoroutine(FadeIn());
        
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

        if (difficultyMenu == null){
            Debug.LogWarning("Difficulty Menu not found in the scene: " + scene.name);
        } else {
            difficultyMenu.SetActive(false); // Ensure Difficulty Menu is inactive
            Debug.Log("Difficulty Menu found and set to inactive");
        }
    
        kills = PlayerPrefsManager.LoadKills();
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

        if (Input.GetKeyDown(KeyCode.K)){
            ToggleDifficultyMenu(); 
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
            GameObject.Find("Player").GetComponent<PlayerController>().canMove = !isPaused;
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

    public void ToggleDifficultyMenu(){
        if (difficultyMenu != null){
            isdifficultyMenuActive = !isdifficultyMenuActive; 
            difficultyMenu.SetActive(isdifficultyMenuActive);
        }
        else{
            Debug.LogWarning("Difficulty Menu is missing"); 
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
        TogglePauseMenu();
    }
    // For the sign out menu to pop up 
    public void SignOutMenu()
    {
        TogglePauseMenu();
        // Quit the game and sign out 
        if (signoutMenu != null)
        {
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

        lastScene = SceneManager.GetActiveScene().name;
        if (lastScene == "Dungeon") {
            PlayerPrefsManager.SetLastScene("Room");
            PlayerPrefsManager.SetCoords(float.PositiveInfinity, float.PositiveInfinity);
        } else {
            PlayerPrefsManager.SetLastScene(lastScene);
            PlayerPrefsManager.SetCoords(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y);
        }
        PlayerPrefsManager.SetKills(kills);
        SceneManager.LoadScene("Start");
        ResetKillCount(); // Reset kill count when the game restarts
    }

    private void ResetKillCount()
    {
        // Reset the kill count and update the kill text
        kills = 0;
        PlayerPrefsManager.SetKills(0);
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
}

