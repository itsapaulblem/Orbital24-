using System.Collections.Generic;
using UnityEngine;

public class StatsManager
{
    private static StatsManager playerInstance;
    private Dictionary<string, float> originalValues = new Dictionary<string, float>();

    // Default Player Stats
    private const float MOVESPEED = 4f;
    private const float MAXHEALTH = 100f;
    private const float ATTACK = 10f;
    private const float ATTACKSPEED = 0.9f;
    private const float BULLETLIFE = 12f;
    private const float BULLETSPEED = 6f;

    private float moveSpeed;
    private float maxHealth;
    private float currentHealth;
    private float attack;
    private float attackSpeed;
    private float bulletLife;
    private float bulletSpeed;

    private StatsManager(float mvSpd, float maxHp, float atk, float atkSpd, float bulLife, float bulSpd)
    {
        moveSpeed = mvSpd;
        maxHealth = maxHp;
        currentHealth = maxHealth;
        attack = atk;
        attackSpeed = atkSpd;
        bulletLife = bulLife;
        bulletSpeed = bulSpd;
    }

    public static StatsManager of(float mvSpd, float maxHp, float atk,
        float atkSpd = -1, float bulLife = -1, float bulSpd = -1)
    {
        return new StatsManager(mvSpd, maxHp, atk, atkSpd, bulLife, bulSpd);
    }

    public static StatsManager ofPlayer(float mvSpd = -1, float maxHp = -1,
        float atk = -1, float atkSpd = -1, float bulLife = -1, float bulSpd = -1)
    {
        // if no existing playerInstance
        if (playerInstance == null)
        {
            mvSpd = mvSpd == -1 ? MOVESPEED : mvSpd;
            maxHp = maxHp == -1 ? MAXHEALTH : maxHp;
            atk = atk == -1 ? ATTACK : atk;
            atkSpd = atkSpd == -1 ? ATTACKSPEED : atkSpd;
            bulLife = bulLife == -1 ? BULLETLIFE : bulLife;
            bulSpd = bulSpd == -1 ? BULLETSPEED : bulSpd;
            playerInstance = new StatsManager(mvSpd, maxHp, atk, atkSpd, bulLife, bulSpd);
        }
        return playerInstance;
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

    public float damage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        return currentHealth / maxHealth;
    }

    public float heal(float healing)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healing);
        return currentHealth / maxHealth;
    }

    public bool isFullHp()
    {
        return currentHealth == maxHealth;
    }

    public bool isDead()
    {
        return currentHealth == 0;
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

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public void IncreaseStat(string stat, float amount)
    {
        switch (stat)
        {
            case "maxHealth":
                RememberOriginalValue(stat, maxHealth);
                maxHealth += amount;
                break;
            case "bulletLife":
                RememberOriginalValue(stat, bulletLife);
                bulletLife += amount;
                break;
            case "moveSpeed":
                RememberOriginalValue(stat, moveSpeed);
                moveSpeed += amount;
                break;
            case "attack":
                RememberOriginalValue(stat, attack);
                attack += amount;
                break;
            case "bulletSpeed":
                RememberOriginalValue(stat, bulletSpeed);
                bulletSpeed += amount;
                break;
            default:
                Debug.LogWarning("Stat not recognised: " + stat);
                break;
        }
    }

    public void RevertStat(string stat)
    {
        if (originalValues.ContainsKey(stat))
        {
            switch (stat)
            {
                case "maxHealth":
                    maxHealth = originalValues[stat];
                    break;
                case "bulletLife":
                    bulletLife = originalValues[stat];
                    break;
                case "moveSpeed":
                    moveSpeed = originalValues[stat];
                    break;
                case "attack":
                    attack = originalValues[stat];
                    break;
                case "bulletSpeed":
                    bulletSpeed = originalValues[stat];
                    break;
                default:
                    Debug.LogWarning("Stat not recognised: " + stat);
                    break;
            }
            originalValues.Remove(stat);
        }
    }

    private void RememberOriginalValue(string stat, float originalValue)
    {
        if (!originalValues.ContainsKey(stat))
        {
            originalValues.Add(stat, originalValue);
        }
    }
}
