using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController class handles the movement, 
/// health and combat of the player.
/// </summary>
public class PlayerController : MonoBehaviour
{   
    // Movement Attributes
    private float movementSpeed = 4f;
    private Vector2 movement;
    private Rigidbody2D rb;

    // Animation Attributes
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRenderer;

    // Combat Attributes
    private string bulletPrefab = "Prefab/Bullet";
    private float maxHealth = 100f;
    public float currentHealth;
    private GameObject healthBar;
    private float maxHealthBarScale;
    private float healRate = 7f;
    private float iFrame = 0.3f;
    private float lastDamageTick;
    private float bulletSpeed = 6f;
    private float bulletLife = 12f;
    public float attack = 10f;
    private bool shootContinuous;
    private bool shootSingle;
    private float timeBetweenShots = 0.9f;
    private float lastFireTime;

    public bool canMove; 
    public GameObject GameOverMenu;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        // Initialise HealthBar
        healthBar = System.Array.Find(gameObject.GetComponentsInChildren<Transform>(), 
                        p => p.gameObject.name == "HealthBar").gameObject;
        maxHealthBarScale = maxHealth / 50;
        healthBar.transform.localScale = new Vector3(maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3(0f, 1f ,1f);
    }

    /// <summary>
    /// UpdatePlayerFacingDirection for animation and Move player based on
    /// player inputs
    /// </summary>
    private void FixedUpdate() 
    {
        UpdatePlayerFacingDirection();
        Move();
    }

    /// <summary>
    /// When player triggers movement inputs, OnMove sets direction of motion
    /// and triggers playerAnimation to transition to moving animation.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnMove(InputValue inputValue) 
    {
        if (!canMove){
            return;
        }
        movement = inputValue.Get<Vector2>();
        movement.Normalize();
        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetFloat("moveY", movement.y);
    }

    private void Move() 
    {
        if (!canMove){
            return;
        }
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Updates sprite facing direction to mousePos.
    /// </summary>
    private void UpdatePlayerFacingDirection() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        playerSpriteRenderer.flipX = mousePos.x < playerPos.x;
    }

    /// <summary>
    /// Update function handles shooting mechanism, independent of movement mechanism. 
    /// If fireButton is pressed, player shootContinuous, with timeBetweenShots
    /// delay (reload) between each shot. If player stops pressing fireButton while 
    /// still reloading, player will shootSingle once reloaded.
    /// </summary>
    private void Update() 
    {
        float timeSinceLastFire = Time.time - lastFireTime;
        if (shootContinuous || shootSingle)
        {
            if (timeSinceLastFire >= timeBetweenShots)
            {
                FireBullet();
                lastFireTime = Time.time;
                shootSingle = false;
            }
        }
        Color temp = healthBar.GetComponent<SpriteRenderer>().color;
        // Heal out of combat
        float timeSinceCombat = Mathf.Min(Time.time - lastDamageTick,timeSinceLastFire);
        if (currentHealth != maxHealth && timeSinceCombat >= timeBetweenShots)
        {
            Heal(healRate*Time.deltaTime);
        }
        else if (currentHealth == maxHealth && temp.a > 0f) {
            temp.a -= 1f * Time.deltaTime;
            healthBar.GetComponent<SpriteRenderer>().color = temp;
        }

        
    }

    /// <summary>
    /// Obtains mousePos and instantiate a bullet to fire at angle relative to 
    /// player position
    /// </summary>
    private void FireBullet()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        Vector2 bulletDir = mousePos - playerPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(Resources.Load<GameObject>(bulletPrefab), transform.position, Quaternion.Euler(0, 0, bulletAngle));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetInit(true, "shot_main", attack, bulletLife, bulletSpeed, bulletDir); // initialise bullet
    }

    /// <summary>
    /// When player triggers firing inputs, OnFire sets player into
    /// shootContinuous mode.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnFire(InputValue inputValue)
    {
        shootContinuous = inputValue.isPressed;
        if (inputValue.isPressed)
        {
            shootSingle = true;
        }
    }

    public void TakeDamage(float damage)
    {
        float timeSinceLastDamage = Time.time - lastDamageTick;
        if (timeSinceLastDamage >= iFrame)
        {
            if (currentHealth == maxHealth) {
                Color temp = healthBar.GetComponent<SpriteRenderer>().color;
                temp.a = 1f;
                healthBar.GetComponent<SpriteRenderer>().color = temp;
            }
            lastDamageTick = Time.time;
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            Vector3 healthBarChange = new Vector3(currentHealth/maxHealth * maxHealthBarScale, 0.1f, 1f);
            healthBar.transform.localScale = healthBarChange;

            if (currentHealth == 0f) {
                Destroy(gameObject); 
                GameOverMenu.SetActive(true);
            }
        }
    }

    public void Heal(float healing)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healing);
        Vector3 healthBarChange = new Vector3(currentHealth/maxHealth * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;
    }
}
