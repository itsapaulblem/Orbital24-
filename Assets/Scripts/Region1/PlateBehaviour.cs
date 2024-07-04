using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBehavior : MonoBehaviour
{
    private static List<PlateBehavior> plates = new List<PlateBehavior>(); 
    private int obstaclesCount = 0; 
    private bool puzzleSolved = false; 

    void Start()
    {
        // Add this plate to the static list of plates 
        plates.Add(this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Increment the count of obstacles touching this plate
            obstaclesCount++;
            // Check puzzle state when an obstacle touches the plate
            CheckPuzzleState(); 
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Decrement the count of obstacles touching this plate
            obstaclesCount--;
            // Puzzle should not be considered solved if an obstacle leaves
            puzzleSolved = false; 
        }
    }

    private void CheckPuzzleState()
    {
        if (!puzzleSolved && obstaclesCount == 3)
        {
            // Notify Puzzle1 script to check puzzle state
            Puzzle1 puzzle = FindObjectOfType<Puzzle1>();
            if (puzzle != null)
            {
                puzzle.CheckPuzzleState(); 
            }
            // Mark puzzle as solved to prevent multiple triggers
            puzzleSolved = true; 
        }
    }
   
    public static bool AllPlatesTriggered()
    {
        foreach (PlateBehavior plate in plates)
        {
            // If any plate still has obstacles not touching it, return false
            if (plate.obstaclesCount < 3)
            {
                return false;
            }
        }
        return true; 
    }

    public bool IsTriggered()
    {
        return obstaclesCount == 3;
    }
}
