using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB3_AI : RangedEnemyAI
{

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Check if undefeated, or if game completed, else destroy
        SetInit(2f, 180f, 18f, 1f, 14f, 8f);
        bulletPrefab = "Prefab/ExBullet";
        lastFireTime = Time.time - stats.GetAttackSpeed();
        audioManager = AudioManager.Instance;
    }

    protected override void FireBullet()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 originPos = transform.position;
        Vector2 bulletDir = playerPos - originPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
        if (audioManager == null) { audioManager = AudioManager.Instance; }
        audioManager.PlaySFX(audioManager.bossThree); // Play hit sound effect
        GameObject bullet = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetInit(false, "shot_bomb", 
                            stats.GetAttack(), 
                            stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), 
                            bulletDir); // initialise bullet
    }
}
