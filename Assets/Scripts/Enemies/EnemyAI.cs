using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System; 

public class EnemyAI : MonoBehaviour
{
    // Reference to audio manager, for enemy SFX
    protected AudioManager audioManager;
    protected StatsManager stats;
    
    // Reference to particle system which will be played upon enemy death
    [SerializeField] private ParticleSystem deathParticlesPrefab = default;
    private ParticleSystem deathParticlesInstance;

    protected enum State { Roaming, Seeking }
    protected State state;

    // Movement Attributes
    protected Rigidbody2D rb;
    protected Vector2 origin;
    protected Vector2 movement;

    // for Seeking
    [SerializeField] protected float sight = 12f;
    protected GameObject player;

    // Animation Attributes
    protected SpriteRenderer enemySpriteRenderer;

    // Combat Attributes
    private GameObject healthBar;
    private float maxHealthBarScale;
    [SerializeField] private float flashDuration = 0.2f; 
    [SerializeField] private Color flashColor = Color.red;

    // Coin prefab 
    [SerializeField] private GameObject coinPrefab;

    // Event to notify NPC of enemy death
    public static event Action EnemyDied;
  

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        
        // Initialize to Roam
        state = State.Roaming;

        // Find Target in Scene
        player = GameObject.Find("Player");

        // Initialize movement
        rb = GetComponent<Rigidbody2D>();
        origin = rb.position;

        // Get renderer
        enemySpriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize HealthBar
        healthBar = System.Array.Find(gameObject.GetComponentsInChildren<Transform>(),
                        p => p.gameObject.name == "HealthBar").gameObject;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);

        SetInit(100f, 200f, 20f); // default initialize
    }

    /// Initialise enemy stats
    public void SetInit(float mvSpd, float maxHp, float atk, 
        float atkSpd = -1, float bulLife = -1, float bulSpd = -1)
    {
     
        // Initialise StatsManager based on provided stats
        stats = StatsManager.of(mvSpd, maxHp, atk, atkSpd, bulLife, bulSpd);

        // Set starting length of healthbar
        maxHealthBarScale = stats.GetMaxHealth() / 50;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3(0f, 1f, 1f);

        // Start async movement routine
        StartCoroutine(MovementRoutine());
    }

    /// Async MovementRoutine retrieves next movement position
    /// based on enemy state
    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                if (state == State.Roaming)
                {
                    movement = GetRoamingPosition();
                    yield return new WaitForSeconds(3f);   
                }
                if (state == State.Seeking)
                {
                    movement = GetSeekingPosition();
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// Default Roaming behaviour: enemy will move within a fixed radius
    /// around the origin point
    protected virtual Vector2 GetRoamingPosition()
    {
        return (origin + new Vector2(UnityEngine.Random.Range(-2f, 2f),UnityEngine.Random.Range(-2f, 2f)) - rb.position).normalized / 2;
    }

    protected virtual Vector2 GetSeekingPosition()
    {
        return (player.transform.position - transform.position).normalized;
    }

    protected virtual void UpdateEnemyFacingDirection()
    {
        enemySpriteRenderer.flipX = movement.x > 0;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // tracks player distance and update enemy state
            state = Vector2.Distance(player.transform.position, rb.position) <= sight
                ? State.Seeking
                : State.Roaming;

            // move enemy based on movement position (from movement routine)
            rb.MovePosition(rb.position + movement * stats.GetMoveSpeed() * Time.fixedDeltaTime);
            UpdateEnemyFacingDirection();
        }
    }

    /// Apply damage to enemy
    public void TakeDamage(float damage)
    {
        // update healthbar to reflect damage taken
        Vector3 healthBarChange = new Vector3(stats.damage(damage) * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;

        // check if enemy is dead
        if (stats.isDead())
        {
            audioManager.PlaySFX(audioManager.enemyDied); // Play death sound effect
            // Instantiate the death particles
            if (deathParticlesPrefab != null)
            {
                deathParticlesInstance = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
                deathParticlesInstance.Play();

                // Detach the particle system from the enemy GameObject
                deathParticlesInstance.transform.parent = null;

                // Destroy the particle system object after the duration of the particle effect
                Destroy(deathParticlesInstance.gameObject, deathParticlesInstance.main.duration);
            }
            // instantiate coin object so that a coin appears when the enemies dies 
            if (coinPrefab != null){
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            // Notify listeners that the enemy has died
            if (EnemyDied != null)
            {
                EnemyDied();
                Debug.Log("Enemy died");
            }
            GameManager.Instance.AddKill();
            
            // Destroy the enemy game object
            Destroy(gameObject);
        } else {    
            // If enemy not dead
            StartCoroutine(FlashEffect());
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.enemybeingshot); // Play hit sound effect
        }
    }

    private IEnumerator FlashEffect()
    {
        Color originalColor = enemySpriteRenderer.color;
        enemySpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        enemySpriteRenderer.color = originalColor;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(stats.GetAttack());
        }
    }
}
