using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBehavior : MonoBehaviour
{
    private static List<PlateBehavior> plates = new List<PlateBehavior>(); 
    private int obstaclesCount = 0; 
    private bool puzzleSolved = false;
    [SerializeField] private GameObject rewardObject; 
    void Start()
    {
        // Add this plate to the static list of plates 
        plates.Add(this);
        Debug.Log("Plate added to list: " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Increment the count of obstacles touching this plate
            obstaclesCount++;
            Debug.Log("Obstacle entered: " + other.name + ", Plate: " + gameObject.name + ", Obstacles count: " + obstaclesCount);
            gameObject.transform.Find("Activated").gameObject.SetActive(true);
            // Check puzzle state when an obstacle touches the plate
            CheckPuzzleState(); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Decrement the count of obstacles touching this plate
            obstaclesCount--;
            Debug.Log("Obstacle exited: " + other.name + ", Plate: " + gameObject.name + ", Obstacles count: " + obstaclesCount);
            gameObject.transform.Find("Activated").gameObject.SetActive(false);
            // Puzzle should not be considered solved if an obstacle leaves
            puzzleSolved = false; 
        }
    }

    private void CheckPuzzleState()
    {
        if (!puzzleSolved && obstaclesCount == 3)
        {
            Debug.Log("Checking puzzle state for Plate: " + gameObject.name);
            Puzzle1 puzzle = FindObjectOfType<Puzzle1>();
            if (puzzle != null)
            {
                Debug.Log("Puzzle found, checking its state...");
                puzzle.CheckPuzzleState(); 
            }
            else
            {
                Debug.LogError("Puzzle1 script not found in the scene.");
            }
            // Mark puzzle as solved to prevent multiple triggers
            puzzleSolved = true; 
        }
    }
   
    public static bool AllPlatesTriggered()
    {
        foreach (PlateBehavior plate in plates)
        {
            Debug.Log("Plate: " + plate.gameObject.name + ", Obstacles: " + plate.obstaclesCount);
            // If any plate still has obstacles not touching it, return false
            if (plate.obstaclesCount < 3)
            {
                return false;
            }
        }
        return true; 
    }
}
