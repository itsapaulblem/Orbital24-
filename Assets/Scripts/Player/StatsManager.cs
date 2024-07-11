using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager
{
    private static StatsManager playerInstance;
    private Dictionary<string, Coroutine> statCoroutines = new Dictionary<string, Coroutine>(); // Coroutine references for stat increases
    private Dictionary<string, int> increaseDuration = new Dictionary<string, int>(); // duration intervals left
    private Dictionary<string, float> originalStat = new Dictionary<string, float>(); // original before increase

    // Default Player Stats
    private const float MOVESPEED = 4f;
    private const float MAXHEALTH = 50f;
    private const float ATTACK = 6f;
    private const float BULLETLIFE = 12f;
    private const float BULLETSPEED = 6f;
    private float currentHealth;
    private float moveSpeed;
    private float maxHealth;
    private float attack;
    private float attackSpeed;
    private float bulletLife;
    private float bulletSpeed;

    // Temporary booster duration
    private float boosterInterval = 100f;
    private Dictionary<string, Color> statFlashColors = new Dictionary<string, Color>(){
        {"moveSpeed", Color.grey},
        {"maxHealth", Color.black},
        {"attack", Color.blue},
        {"bulletLife", Color.yellow},
        {"bulletSpeed", Color.cyan}
    };

    private StatsManager(float mvSpd, float maxHp, float atk, float atkSpd, float bulLife, float bulSpd)
    {
        moveSpeed = mvSpd;
        maxHealth = maxHp;
        currentHealth = maxHp;
        attack = atk;
        attackSpeed = atkSpd; 
        bulletLife = bulLife;
        bulletSpeed = bulSpd;
    }

    public static StatsManager of(float mvSpd, float maxHp, float atk, float atkSpd, float bulLife = -1, float bulSpd = -1)
    {
        return new StatsManager(mvSpd, maxHp, atk, atkSpd, bulLife, bulSpd);
    }

    public static StatsManager ofPlayer(float mvSpd = -1, float maxHp = -1, float atk = -1, float atkSpd = 1, float bulLife = -1, float bulSpd = -1)
    {
        // if no existing playerInstance
        if (playerInstance == null)
        {
            mvSpd = mvSpd == -1 ? MOVESPEED : mvSpd;
            maxHp = maxHp == -1 ? MAXHEALTH : maxHp;
            atk = atk == -1 ? ATTACK : atk;
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

    public float damage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        return currentHealth / maxHealth;
    }

    public float heal(float healing)
    {
        if (healing == -1) {
            currentHealth = maxHealth;
        } else {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healing);
        }
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

    // Increase stat method with coroutine
    public void TemporaryIncreaseStat(string stat, float amount)
    {
        if (!increaseDuration.ContainsKey(stat)) { increaseDuration.Add(stat, 0); }
        increaseDuration[stat] += 1;
        if (increaseDuration[stat] == 1)
        {
            Coroutine coroutine = CoroutineManager.Instance.StartCoroutine(IncreaseStatWithDuration(stat, amount));
        }
    }

    private IEnumerator IncreaseStatWithDuration(string stat, float amount)
    {
        float originalValue = GetStatValue(stat);
        float newValue = originalValue + amount;

        SetStatValue(stat, newValue);

        Coroutine coroutine = CoroutineManager.Instance.StartCoroutine(FlashingIndicator(stat));
        while (increaseDuration[stat] > 0) {
            yield return new WaitForSeconds(boosterInterval);
            increaseDuration[stat] -= 1;
            Debug.Log("One interval complete");
        }

        SetStatValue(stat, originalValue);
    }

    private IEnumerator FlashingIndicator(string stat)
    {
        Renderer playerRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
        Color originalColor = playerRenderer.material.color;
         if (!statFlashColors.TryGetValue(stat, out Color flashColor))
        {
            flashColor = Color.white; // Default flash color if not found
        }
        Debug.Log($"Flashing {stat} with color {flashColor}");
        float flashDuration = 0.5f; // Duration for each flash

        while (increaseDuration[stat] > 0)
        {
            playerRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        playerRenderer.material.color = originalColor;
    }

    // Helper methods
    private float GetStatValue(string stat)
    {
        switch (stat)
        {
            case "moveSpeed":
                return moveSpeed;
            case "maxHealth":
                return maxHealth;
            case "attack":
                return attack;
            case "bulletLife":
                return bulletLife;
            case "bulletSpeed":
                return bulletSpeed;
            default:
                Debug.LogWarning("Stat not recognised: " + stat);
                return 0f;
        }
    }

    private void SetStatValue(string stat, float value)
    {
        switch (stat)
        {
            case "moveSpeed":
                moveSpeed = value;
                break;
            case "maxHealth":
                maxHealth = value;
                break;
            case "attack":
                attack = value;
                break;
            case "bulletLife":
                bulletLife = value;
                break;
            case "bulletSpeed":
                bulletSpeed = value;
                break;
            default:
                Debug.LogWarning("Stat not recognised: " + stat);
                break;
        }
    }
}
