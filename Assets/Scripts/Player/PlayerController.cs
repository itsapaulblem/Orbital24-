using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

/// PlayerController class handles the movement, 
/// health and combat of the player.
public class PlayerController : MonoBehaviour
{   
    private static float xCoord = float.PositiveInfinity;
    private static float yCoord = float.PositiveInfinity;
    private AudioManager audioManager;
    private StatsManager stats;

    // Movement Attributes
    private Vector2 movement;
    private Rigidbody2D rb;
    public bool canMove = true;

    // Animation Attributes
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRenderer;

    // Combat Attributes
    private string bulletPrefab = "Prefab/Bullet";
    private GameObject healthBar;
    private float maxHealthBarScale;
    private float healRate = 7f;
    private float iFrame = 0.3f;
    private float lastDamageTick;
    private bool shootContinuous;
    private bool shootSingle;
    //private float timeBetweenShots = 0.9f;
    private float lastFireTime;

    public GameObject GameOverMenu;
    [SerializeField] private float flashDuration = 0.2f; 
    [SerializeField] private Color flashColor = Color.red; 


    private void Awake() 
    {
        audioManager = AudioManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        // TODO: retrieve stats data from Firebase, generate using .ofPlayer(stats)
        stats = StatsManager.ofPlayer();

        // Initialise HealthBar
        healthBar = System.Array.Find(gameObject.GetComponentsInChildren<Transform>(), 
                        p => p.gameObject.name == "HealthBar").gameObject;
        maxHealthBarScale = stats.GetMaxHealth() / 50;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3(0f, 1f ,1f);

        if (xCoord != float.PositiveInfinity && yCoord != float.PositiveInfinity) {
            transform.position = new Vector2(xCoord, yCoord);
            xCoord = float.PositiveInfinity;
            yCoord = float.PositiveInfinity;
        }
    }
    private void Start(){
        audioManager = AudioManager.Instance;
    }

    public static void SetCoords(float x, float y) {
        xCoord = x;
        yCoord = y;
    }

    /// UpdatePlayerFacingDirection for animation and Move player based on
    /// player inputs
    private void FixedUpdate() 
    {
        if (canMove) {
            UpdatePlayerFacingDirection();
            Move();
        }
    }

    /// When player triggers movement inputs, OnMove sets direction of motion
    /// and triggers playerAnimation to transition to moving animation.
    ///   inputValue - player inputs
    private void OnMove(InputValue inputValue) 
    {
        if (!canMove) return;
        movement = inputValue.Get<Vector2>();
        movement.Normalize();
        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetFloat("moveY", movement.y);
    }

    private void Move() 
    {
        
        rb.MovePosition(rb.position + movement * stats.GetMoveSpeed() * Time.fixedDeltaTime);
    }

    /// Updates sprite facing direction to mousePos.
    private void UpdatePlayerFacingDirection() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        playerSpriteRenderer.flipX = mousePos.x < playerPos.x;
    }

    /// Update function handles shooting mechanism, independent of movement mechanism. 
    /// If fireButton is pressed, player shootContinuous, with timeBetweenShots
    /// delay (reload) between each shot. If player stops pressing fireButton while 
    /// still reloading, player will shootSingle once reloaded.
    public int x = 1;
    private void Update()
    {
        float timeSinceLastFire = Time.time - lastFireTime;

        if (canMove && (shootContinuous || shootSingle))
        {
            if (timeSinceLastFire >= stats.GetAttackSpeed())
            {
                FireBullet();
                lastFireTime = Time.time;
                shootSingle = false; // Reset shootSingle after firing
            }
        }

        // Heal out of combat
        float timeSinceCombat = Mathf.Min(Time.time - lastDamageTick, timeSinceLastFire);
        Color temp = healthBar.GetComponent<SpriteRenderer>().color;

        if (!stats.isFullHp() && timeSinceCombat >= stats.GetAttackSpeed())
        {
            Heal(healRate * Time.deltaTime);
        }
        else if (stats.isFullHp() && temp.a > 0f)
        {
            temp.a -= 1f * Time.deltaTime;
            healthBar.GetComponent<SpriteRenderer>().color = temp;
        }
    }

    /// Obtains mousePos and instantiate a bullet to fire at angle relative to 
    /// player position
    private void FireBullet()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        Vector2 bulletDir = mousePos - playerPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(Resources.Load<GameObject>(bulletPrefab), transform.position, Quaternion.Euler(0, 0, bulletAngle));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetInit(true, "shot_main",     // by player, shot_main sprite
                            stats.GetAttack(), 
                            stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), 
                            bulletDir); // initialise bullet
     
        if (audioManager != null) {
            audioManager.PlaySFX(audioManager.bobshooting); // Play shooting sound effect
        }

    }

    /// When player triggers firing inputs, OnFire sets player into
    /// shootContinuous mode.
    ///   inputValue - player inputs
    private void OnFire(InputValue inputValue)
    {
        if (!canMove) return;
        if (inputValue.isPressed) {
            shootSingle = true;
        }
        shootContinuous = inputValue.isPressed;
    }

    /// Apply damage to player
    public void TakeDamage(float damage)
    {
        float timeSinceLastDamage = Time.time - lastDamageTick;
        if (timeSinceLastDamage >= iFrame)
        {
            if (stats.isFullHp()) {
                Color temp = healthBar.GetComponent<SpriteRenderer>().color;
                temp.a = 1f;
                healthBar.GetComponent<SpriteRenderer>().color = temp;
            }
            lastDamageTick = Time.time;
            Vector3 healthBarChange = new Vector3(stats.damage(damage) * maxHealthBarScale, 0.1f, 1f);
            healthBar.transform.localScale = healthBarChange;

            if (stats.isDead()) {
                Destroy(gameObject); 
                audioManager.PlaySFX(audioManager.bobdied); // Play shooting sound effect
                GameManager.Instance.GameOver(); // calling GameOver from GameManager 
            }
            else {
                StartCoroutine(FlashEffect());
                audioManager.PlaySFX(audioManager.bobbeingshot);
            }
        }
    }

    /// Apply healing to player
    public void Heal(float healing)
    {
        maxHealthBarScale = stats.GetMaxHealth() / 50;
        Vector3 healthBarChange = new Vector3(stats.heal(healing) * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;
    }

      private IEnumerator FlashEffect()
    {
        Color originalColor = playerSpriteRenderer.color;
        playerSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        playerSpriteRenderer.color = originalColor;
    }
}
