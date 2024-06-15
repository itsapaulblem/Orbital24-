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
    private float temptime = 10f;
    private float templast;
    

    private void Start()
    {
        StartCoroutine(Spawner());
        player = GameObject.Find("Player");
        templast = Time.time - temptime;
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (true) {
            if (enemies.Count >= enemyCap) {
                enemies.RemoveAll(e => e == null);
            }
            if (enemies.Count < enemyCap) {
                int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length); // Changed here
                GameObject enemyToSpawn = enemyPrefabs[rand];

                Vector2 spawnLocation = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * 2;

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
