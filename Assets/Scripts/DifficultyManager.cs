using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public void SetDifficulty(int difficulty){
        switch(difficulty){
            case 0:
                StatsManager.difficulty = StatsManager.Difficulty.Easy;
                break; 
            case 1:
                StatsManager.difficulty = StatsManager.Difficulty.Medium;
                break;
            case 2:
                StatsManager.difficulty = StatsManager.Difficulty.Hard;
                break;
            default:
                StatsManager.difficulty = StatsManager.Difficulty.Medium;
                break;
        }
        GameObject.Find("Town Background").GetComponent<SceneChanger>().waiting = false; 
    }
}
