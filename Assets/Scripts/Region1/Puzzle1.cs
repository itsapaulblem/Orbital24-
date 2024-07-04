using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle1 : MonoBehaviour
{
    public GameObject rewardObject;
    // specify the reward spawn position 
    public Vector3 spawnPosition;
    private bool puzzleSolved = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize puzzle state
        puzzleSolved = false;
    }

    public void CheckPuzzleState()
    {
        if (!puzzleSolved && PlateBehavior.AllPlatesTriggered())
        {
            // if puzzle solved, spawn the reward object
            spawnReward();

            // mark puzzle as solved to prevent spawning multiple times
            puzzleSolved = true;
        }
    }

    private void spawnReward()
    {
        Instantiate(rewardObject, spawnPosition, Quaternion.identity);
    }
}
