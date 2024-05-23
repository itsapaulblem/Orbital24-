using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueBeginning : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("DialogueBeginning");
    }
}
