using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    private enum State { Roaming, Seeking }
    private State state;

    // Movement Attributes
    public float movementSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 origin;
    private Vector2 movement;
    // for Seeking
    [SerializeField] private float sight = 10f;
    private GameObject player;

    // Animation Attributes
    private SpriteRenderer enemySpriteRenderer;

    // Combat Attributes
    private float maxHealth = 30f;
    public float currentHealth;
    private GameObject healthBar;
    private float maxHealthBarScale;
    private float attack = 3f;
    
    
    private void Awake()
    {
        state = State.Roaming;
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        origin = rb.position;
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        
        // Initialise HealthBar
        healthBar = System.Array.Find(gameObject.GetComponentsInChildren<Transform>(), 
                        p => p.gameObject.name == "HealthBar").gameObject;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        SetMaxHealth(30); // default 30 hp 
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        maxHealthBarScale = maxHealth / 50;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3(0f, 1f ,1f);
    }

    private void Start()
    {
        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                movement = GetRoamingPosition();
                yield return new WaitForSeconds(3f); // Add a delay to prevent infinite loop   
            }
            if (state == State.Seeking)
            {
                movement = GetSeekingPosition();
                yield return new WaitForSeconds(Time.fixedDeltaTime); // Add a delay to prevent infinite loop   
            }
        }
    }

    public virtual Vector2 GetRoamingPosition()
    {
        return (origin + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) - rb.position).normalized / 2;
    }

    public virtual Vector2 GetSeekingPosition()
    {
        return (player.transform.position - transform.position).normalized;
    }

    private void UpdateEnemyFacingDirection() 
    {
        enemySpriteRenderer.flipX = movement.x > 0;
    }

    private void FixedUpdate()
    {
        state = Vector2.Distance(player.transform.position, rb.position) <= sight
            ? State.Seeking
            : State.Roaming;

        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);   
        UpdateEnemyFacingDirection();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("OnTriggerEnter2D");
        Debug.Log(damage);
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Vector3 healthBarChange = new Vector3(currentHealth/maxHealth * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;

        if (currentHealth == 0f) {
            Destroy(gameObject);
            // SceneManager.LoadScene("RewardScene");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(attack);
        }
    }
}
