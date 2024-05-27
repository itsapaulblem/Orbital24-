using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Image healthBar;

    private NPC_Mermaid npcMermaid;
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
    [SerializeField] private int maxHealth = 100; // Added maxHealth variable
    private int currentHealth; // Current health of the enemy

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        if (enemyPathfinding == null)
        {
            Debug.LogError("EnemyPathfinding component not found!");
        }

        state = State.Roaming;
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found!");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found!");
        }

        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        // Initialize current health
        currentHealth = maxHealth;
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

            yield return new WaitForSeconds(2f); // Add a delay to prevent infinite loop
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float fillAmount = (float)currentHealth / maxHealth; // Calculate fill amount based on current health
        if (healthBar != null)
        {
            healthBar.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogError("Health bar Image is not assigned!");
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            // add reward scene
        }
    }
}
