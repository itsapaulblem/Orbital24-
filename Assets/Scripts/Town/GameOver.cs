using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOver : MonoBehaviour
{
    public GameObject GameOverMenu;
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
