using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f; 
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private int enemyCap = 3;
    private GameObject player;
    private List<GameObject> enemies = new List<GameObject>();
    private bool canSpawn = false;
    private float spawnSight = 35f; // spawn if player within
    

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update() 
    {
        if (player == null) { return; }
        if (Vector2.Distance(player.transform.position, transform.position) <= spawnSight && !canSpawn) {
            canSpawn = true;
            StartCoroutine(Spawner());
        } else if (Vector2.Distance(player.transform.position, transform.position) > spawnSight) {
            canSpawn = false;
            foreach (GameObject e in enemies) {
                if (e != null && Vector2.Distance(player.transform.position, e.transform.position) > spawnSight) {
                    Destroy(e);
                }
            }
            enemies.RemoveAll(e => e == null);
        }
            
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn) {
            if (enemies.Count >= enemyCap) {
                enemies.RemoveAll(e => e == null);
            }
            if (enemies.Count < enemyCap) {
                int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length); // Changed here
                GameObject enemyToSpawn = enemyPrefabs[rand];

                Vector2 spawnLocation = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * 5;

                GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity);

                enemies.Add(spawnedEnemy);

                EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();
                if (enemyAI != null){
                    enemyAI.SetInit(2f, 50f, 3f);
                }
            }
            yield return wait;
        }
    }
}
