using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Seeking
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private GameObject player;
    private Rigidbody2D rb;
    private new Camera camera;
    [SerializeField] private float sight = 20f;
    [SerializeField] private int health = 100; // Added health variable

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        camera = Camera.main;
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
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private Vector2 GetSeekingPosition()
    {
        return (player.transform.position - transform.position).normalized;
    }

    private void Update()
    {
        state = Vector2.Distance(player.transform.position, rb.position) <= sight
            ? State.Seeking
            : State.Roaming;
        if (state == State.Seeking)
        {
            Vector2 roamPos = GetSeekingPosition();
            enemyPathfinding.MoveTo(roamPos);
        }
    
    }

    public void TakeDamage(int damage){
        health -= damage; 
        if (health <= 0 ){
            Destroy(gameObject);
        }
    }
}
