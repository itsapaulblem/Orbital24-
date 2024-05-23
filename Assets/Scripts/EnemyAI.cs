using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State {
        Roaming,
        Seeking
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private GameObject player; 
    private Rigidbody2D rb;
    
    [SerializeField] private float sight = 20f;
    
    private void Awake() 
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() 
    {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine() 
    {
        while (true) 
        {
            if (state == State.Roaming) 
            {
                Vector2 roamPos = GetRoamingPosition();
                enemyPathfinding.MoveTo(roamPos);
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition() 
    {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private Vector2 GetSeekingPosition() {
        return (player.transform.position - transform.position).normalized;
    }

    private void Update()
    {
        state = Vector2.Distance((Vector2)player.transform.position, rb.position) <= sight 
                    ? State.Seeking : State.Roaming;
        if (state == State.Seeking) 
        {
            Vector2 roamPos = GetSeekingPosition();
            enemyPathfinding.MoveTo(roamPos);
        }
    }
}
