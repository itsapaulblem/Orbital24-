using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle1 : MonoBehaviour
{
    public Vector3 spawnPosition; // Specify the reward spawn position
    public GameObject rewardObject; // Reference to the instantiated reward object
    private bool puzzleSolved = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize puzzle state
        puzzleSolved = false;

         // Move the reward object to the spawn position and set it inactive
        rewardObject.transform.position = spawnPosition;
        rewardObject.SetActive(false);
    }

    public void CheckPuzzleState()
    {
        if (!puzzleSolved && PlateBehavior.AllPlatesTriggered())
        {
            // If puzzle solved, activate the reward object
            rewardObject.SetActive(true);

            // Mark puzzle as solved to prevent activating multiple times
            puzzleSolved = true;
        }
    }
}
