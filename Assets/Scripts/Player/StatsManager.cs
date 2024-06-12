using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private static StatsManager instance;
    public static StatsManager Instance { get { return instance; } }

    // Player stats
    [SerializeField] private float baseMoveSpeed = 4f;
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseAttack = 10f;
    [SerializeField] private float baseAttackSpeed = 0.9f;
    [SerializeField] private float baseBulletLife = 12f;
    [SerializeField] private float baseBulletDamage = 10f; 

    private float moveSpeed;
    private float maxHealth;
    private float attack;
    private float attackSpeed;
    private float bulletLife;
    private float bulletDamage; 

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Initialize stats with base values
        moveSpeed = baseMoveSpeed;
        maxHealth = baseMaxHealth;
        attack = baseAttack;
        attackSpeed = baseAttackSpeed;
        bulletLife = baseBulletLife;
        bulletDamage = baseBulletDamage; 
    }

    // Getter and setter methods for player stats
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
    }

    public float GetAttack()
    {
        return attack;
    }

    public void SetAttack(float value)
    {
        attack = value;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public void SetAttackSpeed(float speed)
    {
        attackSpeed = speed;
    }

    public float GetBulletLife()
    {
        return bulletLife;
    }

    public void SetBulletLife(float life)
    {
        bulletLife = life;
    }

    public float GetBulletDamage(){
        return bulletDamage; 
    }

    public void SetBulletDamage(float damage){
        bulletDamage = damage; 
    }
}
