using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Reference to the rate at which enemies spawn (in seconds)
    [SerializeField] private float spawnRate = 1f; 
    // Reference to array of enemy prefabs to spawn 
    [SerializeField] private GameObject[] enemyPrefabs; 
    // Reference to maximum number of enemies allowed to be spawned at any time 
    [SerializeField] private int enemyCap = 3;
    // Reference to the player GameObject 
    private GameObject player;
    // List to keep track of spawned enemies
    private List<GameObject> enemies = new List<GameObject>();
    private bool canSpawn = false;
    // Distance within which the player must be to start spawing enemies 
    private float spawnSight = 35f; 
    

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update() 
    {
        if (player == null) { return; }
        // Check the distance between the player and the spawner 
        if (Vector2.Distance(player.transform.position, transform.position) <= spawnSight && !canSpawn) {
            // If the player is within spawn sight and spawning is not already happening, start spawning 
            canSpawn = true;
            StartCoroutine(Spawner());
        } else if (Vector2.Distance(player.transform.position, transform.position) > spawnSight) {
            // If the player is out of spawn sight, stop spawning 
            canSpawn = false;
            // Destroy enemies that are out of the spawn sight range 
            foreach (GameObject e in enemies) {
                if (e != null && Vector2.Distance(player.transform.position, e.transform.position) > spawnSight) {
                    Destroy(e);
                }
            }
            enemies.RemoveAll(e => e == null);
        }
            
    }
    // Coroutine for spawning enemies at set intervals 
    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn) {
            // If the number of enemies exceeds the cap, clean up null references 
            if (enemies.Count >= enemyCap) {
                enemies.RemoveAll(e => e == null);
            }
            // If there are fewer enemies than the cap, spawn a new one 
            if (enemies.Count < enemyCap) {
                int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length); // Changed here
                GameObject enemyToSpawn = enemyPrefabs[rand];

                Vector2 spawnLocation = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * 5;

                GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity);

                enemies.Add(spawnedEnemy);
                // Initialize the enemy AI if it exists 
                EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();
                if (enemyAI != null){
                    enemyAI.SetInit(2f, 50f, 3f);
                } // TODO: Delete
            }
            yield return wait;
        }
    }
}
