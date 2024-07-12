using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f; 
    private Dictionary<string, Func<Vector2, GameObject>> Spawn;

    // Maximum number of enemies allowed to be spawned at any time 
    [SerializeField] private int enemyCap = 3;
    // Reference to the player GameObject 
    private GameObject player;
    // List to keep track of spawned enemies
    private List<GameObject> enemies = new List<GameObject>();
    private bool canSpawn = false;
    // Distance within which the player must be to start spawing enemies 
    private float spawnSight = 35f; 
    [SerializeField] private float spawnRange = 5; 
    [SerializeField] private int internalDifficulty = 0;
    

    private void Start()
    {
        player = GameObject.Find("Player");
        Spawn = new Dictionary<string, Func<Vector2, GameObject>>() {
            {"Melee", (loc) => {
                GameObject meleePrefab = Resources.Load<GameObject>("Prefab/Enemy");
                GameObject enemy = Instantiate(meleePrefab, loc, Quaternion.identity); 
                SpriteLibrary sl = enemy.GetComponent<SpriteLibrary>();
                sl.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Enemies/Side Animation/m_enemy_1"); 
                float difficultyMod = 1 + internalDifficulty * 0.05f;
                enemy.GetComponent<EnemyAI>().SetInit(2.5f * difficultyMod, 30f * difficultyMod, 6f * difficultyMod);
                return enemy;
            }},
            {"Ranged", (loc) => {
                GameObject rangedPrefab = Resources.Load<GameObject>("Prefab/RangedEnemy");
                GameObject enemy = Instantiate(rangedPrefab, loc, Quaternion.identity); 
                SpriteLibrary sl = enemy.GetComponent<SpriteLibrary>();
                sl.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Enemies/Side Animation/r_enemy_1"); 
                float difficultyMod = 1 + internalDifficulty * 0.05f;
                enemy.GetComponent<EnemyAI>().SetInit(2f * difficultyMod, 20f * difficultyMod, 10f * difficultyMod,
                    0.9f * difficultyMod, 14f * difficultyMod, 12f * difficultyMod);
                return enemy;
            }},
            {"Mixed", (loc) => {
                if (UnityEngine.Random.value < 0.5) { return Spawn["Melee"](loc); }
                else { return Spawn["Ranged"](loc); }
            }}
        };
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
                Vector2 spawnLocation = transform.position + 
                    new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0) * spawnRange;
                GameObject spawnedEnemy = Spawn["Mixed"](spawnLocation);

                enemies.Add(spawnedEnemy);
            }
            yield return wait;
        }
    }
}
