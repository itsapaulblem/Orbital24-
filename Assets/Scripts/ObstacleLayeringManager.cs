using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLayeringManager : MonoBehaviour
{
    static ObstacleLayeringManager instance;

    public static ObstacleLayeringManager Instance 
    {
        get 
        {
            if (instance == null) {
                instance = (ObstacleLayeringManager)FindObjectOfType(typeof(ObstacleLayeringManager));
            }
            return instance;
        }
    }

    [SerializeField] GameObject player;
    public GameObject Player { get{ return player; } }
}
