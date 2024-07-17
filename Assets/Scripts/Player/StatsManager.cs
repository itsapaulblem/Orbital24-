using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager
{
    private static StatsManager playerInstance;
    private Dictionary<string, Coroutine> statCoroutines = new Dictionary<string, Coroutine>(); // Coroutine references for stat increases
    private Dictionary<string, int> boosterCount = new Dictionary<string, int>(); // number of concurrent boosters

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

    // Difficulty Level
    public enum Difficulty { Easy, Medium, Hard }
    public static Difficulty difficulty = Difficulty.Easy;

    // Temporary booster duration
    private float boosterInterval = 150f;
    private Dictionary<string, Color> statFlashColors = new Dictionary<string, Color>(){
        {"moveSpeed", Color.grey},
        {"maxHealth", Color.black},
        {"attack", Color.blue},
        {"bulletLife", Color.yellow},
        {"bulletSpeed", Color.cyan}
    };
    private bool IsFlashing = false;
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
        float difficultyMod = 1f;
        switch (difficulty) {
            case Difficulty.Hard: difficultyMod += 0.5f; break;
            case Difficulty.Medium: difficultyMod += 0.2f; break;
        }
        return new StatsManager(mvSpd * difficultyMod, 
                                maxHp * difficultyMod, 
                                atk * difficultyMod, 
                                atkSpd * difficultyMod, 
                                bulLife * difficultyMod, 
                                bulSpd * difficultyMod);
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
            currentHealth = Mathf.Min(maxHealth, Mathf.Max(currentHealth + healing, 1));
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
        if (!boosterCount.ContainsKey(stat)) { boosterCount.Add(stat, 0); }
        boosterCount[stat] += 1;
        Coroutine coroutine = CoroutineManager.Instance.StartCoroutine(IncreaseStatWithDuration(stat, amount));
    }

    private IEnumerator IncreaseStatWithDuration(string stat, float amount)
    {
        AddStatValue(stat, amount);

        if (!IsFlashing) {
            IsFlashing = true;
            Coroutine coroutine = CoroutineManager.Instance.StartCoroutine(FlashingIndicator());
        }
        yield return new WaitForSeconds(boosterInterval);
        boosterCount[stat] -= 1;
        AddStatValue(stat, -amount);
    }

    private IEnumerator FlashingIndicator()
    {
        Renderer playerRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
        Color originalColor = playerRenderer.material.color;
        
        float flashDuration = 0.5f; // Duration for each flash
        bool flashed = true;
        while (flashed)
        {
            flashed = false;
            foreach (KeyValuePair<string,Color> p in statFlashColors) {
                bool tmp = boosterCount.ContainsKey(p.Key) ? boosterCount[p.Key] > 0 : false;
                playerRenderer.material.color = tmp ? p.Value : originalColor;
                if (tmp) flashed = true;
                yield return new WaitForSeconds(flashDuration);
            }
        }
        IsFlashing = false;
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

    private void AddStatValue(string stat, float value)
    {
        switch (stat)
        {
            case "moveSpeed":
                moveSpeed += value;
                break;
            case "maxHealth":
                maxHealth += value;
                currentHealth += value;
                GameObject.Find("Player").GetComponent<PlayerController>().Heal(value);
                break;
            case "attack":
                attack += value;
                break;
            case "bulletLife":
                bulletLife += value;
                break;
            case "bulletSpeed":
                bulletSpeed += value;
                break;
            default:
                Debug.LogWarning("Stat not recognised: " + stat);
                break;
        }
    }
}
