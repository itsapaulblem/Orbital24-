using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Assuming "LevelOne" is the name of your scene
        SceneManager.LoadSceneAsync("LevelOne");
    }
}
