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
        if (rewardObject != null)
        {
            rewardObject.transform.position = spawnPosition;
            rewardObject.SetActive(false);
            Debug.Log("Reward object set to inactive at position: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Reward object is not assigned.");
        }
    }

    public void CheckPuzzleState()
    {
        Debug.Log("Checking puzzle state...");
        if (!puzzleSolved && PlateBehavior.AllPlatesTriggered())
        {
            Debug.Log("Puzzle solved! Activating reward object.");
            if (rewardObject != null)
            {
                rewardObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Reward object is not assigned.");
            }
            puzzleSolved = true;
        }
        else
        {
            Debug.Log("Puzzle not solved yet or already solved.");
        }
    }
}
