using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalbossAI : RangedEnemyAI
{
    void Start()
    {
        // TODO: Check if undefeated, or if game completed, else destroy
        SetInit(0f, 350f, 15f, 0.8f, 50f, 7f);
        sight = 23f;
        lastFireTime = Time.time - stats.GetAttackSpeed();
        audioManager = AudioManager.Instance;
    }

    protected override Vector2 GetSeekingPosition() { return Vector2.zero; }
    protected override Vector2 GetRoamingPosition() { return Vector2.zero; }
    protected override void UpdateEnemyFacingDirection() { return; }

    protected override void FireBullet()
    {
        if (player == null || stats.isDead()) return;
        if (audioManager == null) { audioManager = AudioManager.Instance; }
        audioManager.PlaySFX(audioManager.bossFinal); // Play hit sound effect

        Quaternion bulletAngle = Quaternion.Euler(0f,0f, UnityEngine.Random.Range(0f,30f));

        Quaternion angleMod = Quaternion.Euler(0f,0f, 30f);

        for (int _ = 0; _ < 12; _++) {
            GameObject bullet = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position,  bulletAngle);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetInit(false, "shot_fire", stats.GetAttack(), stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), bulletAngle * Vector2.right, false);
            
            bulletAngle = bulletAngle * angleMod;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (stats.isDead()) { return; }
        // update healthbar to reflect damage taken
        Vector3 healthBarChange = new Vector3(stats.damage(damage) * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;

        // check if enemy is dead
        if (stats.isDead())
        {
            audioManager.PlaySFX(audioManager.enemyDied); // Play death sound effect
            // Notify listeners that the enemy has died
            TriggerListener();
            GameManager.Instance.AddKill();
        } else {    
            // If enemy not dead
            StartCoroutine(FlashEffect());
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.enemybeingshot); // Play hit sound effect
        }
    }
}
