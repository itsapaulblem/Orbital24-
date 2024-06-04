using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f; 
    [SerializeField] private GameObject[] enemyPrefabs; 

    [SerializeField] private bool canSpawn = true; 

    private void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn)
        {
            yield return wait;  
            int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length); // Changed here
            GameObject enemyToSpawn = enemyPrefabs[rand];
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);

            EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();
            if (enemyAI != null){
                enemyAI.SetInit(50f, 3f, 3f);
            }
        }
    }
}
