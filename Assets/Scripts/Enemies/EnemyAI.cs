using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine.SocialPlatforms.Impl;


public class EnemyAI : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathParticlesPrefab = default;
    private ParticleSystem deathParticlesInstance;

    protected enum State { Roaming, Seeking }
    protected State state;

    // Movement Attributes
    protected float movementSpeed = 2f;
    protected Rigidbody2D rb;
    protected Vector2 origin;
    protected Vector2 movement;

    // for Seeking
    [SerializeField] private float sight = 12f;
    protected GameObject player;

    // Animation Attributes
    protected SpriteRenderer enemySpriteRenderer;

    // Combat Attributes
    private float maxHealth = 30f;
    public float currentHealth;
    private GameObject healthBar;
    private float maxHealthBarScale;
    protected float attack = 3f;

    private void Awake()
    {
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

        SetInit(50f, 3f, 2f); // default initialize
    }

    public void SetInit(float health, float attack, float moveSpeed)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        maxHealthBarScale = maxHealth / 50;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3(0f, 1f, 1f);

        this.attack = attack;
        this.movementSpeed = moveSpeed;
        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (player != null)
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
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    protected virtual Vector2 GetRoamingPosition()
    {
        return (origin + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) - rb.position).normalized / 2;
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
            state = Vector2.Distance(player.transform.position, rb.position) <= sight
                ? State.Seeking
                : State.Roaming;

            rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
            UpdateEnemyFacingDirection();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Vector3 healthBarChange = new Vector3(currentHealth / maxHealth * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;
       

        if (currentHealth == 0f)
        {
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
            GameManager.Instance.AddKill();
            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(attack);
        }
    }

}
