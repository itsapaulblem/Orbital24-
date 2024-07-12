using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBehavior : MonoBehaviour
{
    private static List<PlateBehavior> plates = new List<PlateBehavior>(); 
    private int obstaclesCount = 0; 
    private bool puzzleSolved = false;
    public Vector3 spawnPosition; // Specify the reward spawn position
    public GameObject rewardObject; // Reference to the instantiated reward object

    void Start()
    {
        // Add this plate to the static list of plates 
        plates.Add(this);
        Debug.Log("Plate added to list: " + gameObject.name);
        // Initialize puzzle state
        puzzleSolved = !PlayerPrefsManager.CheckItem("The Blue Bloodstone");

        // Move the reward object to the spawn position and set it inactive
        if (rewardObject != null)
        {
            rewardObject.transform.position = spawnPosition;
            rewardObject.SetActive(false);
            Debug.Log("Reward object set to inactive at position: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Reward object is not assigned or collected.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("Obstacle"))
        // alternative if check
        if (other.name.Contains("Movable Rock") || other.name.Contains("Player"))
        {
            gameObject.transform.Find("Activated").gameObject.SetActive(true);
            // Increment the count of obstacles touching this plate
            obstaclesCount++;
            Debug.Log("Obstacle entered: " + other.name + ", Plate: " + gameObject.name + ", Obstacles count: " + obstaclesCount);
            // Check puzzle state when an obstacle touches the plate
            CheckPuzzleState(); 
        }
    }

    void OnDestroy(){
        // Remove this plate from the static list of plates
        plates.Remove(this);
        Debug.Log("Plate removed from list: " + gameObject.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //if (other.CompareTag("Obstacle"))
        if (other.name.Contains("Movable Rock") || other.name.Contains("Player"))
        {
            // Decrement the count of obstacles touching this plate
            obstaclesCount--;
        }
        if (obstaclesCount == 0){
            gameObject.transform.Find("Activated").gameObject.SetActive(false);
            
            Debug.Log("Obstacle exited: " + other.name + ", Plate: " + gameObject.name + ", Obstacles count: " + obstaclesCount);
        }
    }

    private void CheckPuzzleState()
    {
        if (!puzzleSolved && AllPlatesTriggered())
        {
            Debug.Log("All plates triggered. Spawning reward object at position: " + spawnPosition);
            if (rewardObject != null)
            {
                rewardObject.SetActive(true); // Activate the reward object
            }
            puzzleSolved = true; // Mark puzzle as solved to prevent multiple triggers
        }
    }

    public static bool AllPlatesTriggered()
    {
        foreach (PlateBehavior plate in plates)
        {
            Debug.Log("Plate: " + plate.gameObject.name + ", Obstacles: " + plate.obstaclesCount);
            // If any plate still has obstacles not touching it, return false
            if (plate.obstaclesCount < 1) 
            {
                return false;
            }
        }
        return true; 
    }
}
