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
    private Camera camera;

    [SerializeField] private float sight = 20f;
    [SerializeField] private float screenBorder;

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
        HandleEnemyOffScreen();
    }

    private void HandleEnemyOffScreen()
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        Vector2 targetDirection = Vector2.zero;

        if ((screenPosition.x < screenBorder && rb.velocity.x < 0) || (screenPosition.x > camera.pixelWidth - screenBorder && rb.velocity.x > 0))
        {
            targetDirection = new Vector2(0, rb.velocity.y);
        }
        if ((screenPosition.y < screenBorder && rb.velocity.y < 0) || (screenPosition.y > camera.pixelHeight - screenBorder && rb.velocity.y > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
}
