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
    [SerializeField] private float screenBorder;
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
        HandleEnemyOffScreen();
    }

    private void HandleEnemyOffScreen()
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);

        // Get the screen boundaries
        float minX = screenBorder;
        float maxX = camera.pixelWidth - screenBorder;
        float minY = screenBorder;
        float maxY = camera.pixelHeight - screenBorder;

        // Clamp the enemy's position to stay within the screen boundaries
        float clampedX = Mathf.Clamp(screenPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(screenPosition.y, minY, maxY);
        Vector3 clampedPosition = camera.ScreenToWorldPoint(new Vector3(clampedX, clampedY, screenPosition.z));

        // Update the enemy's position
        transform.position = clampedPosition;
    }

    public void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Bullet")){
            health -= 10;
            if (health <= 0){
                Destroy(gameObject);
                
            }
        }
    }
}
