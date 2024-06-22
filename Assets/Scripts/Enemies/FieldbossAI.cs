using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldbossAI : RangedEnemyAI
{   
    void Start()
    {
        // TODO: Check if undefeated, or if game completed, else destroy
        SetInit(1f, 250, 13f, 1.1f, 15f, 12f);
        sight = 16f;
        lastFireTime = Time.time - stats.GetAttackSpeed();
    }

    protected override Vector2 GetSeekingPosition()
    {
        if (player == null) return Vector2.zero;

        float dist = Vector2.Distance(player.transform.position, transform.position);
        if (dist > distanceToStop) {
            return (player.transform.position - transform.position).normalized;
        } else if (dist < distanceToStop) {
            return (transform.position - player.transform.position).normalized;
        }
        else {
            return Vector2.zero;
        }
    }

    protected override void FireBullet()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 originPos = transform.position;
        Vector2 bulletDir = playerPos - originPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        Quaternion angleMod1 = Quaternion.Euler(0f,0f, -6f);
        Quaternion angleMod3 = Quaternion.Euler(0f,0f, 6f);

        GameObject bullet1 = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle-6));
        GameObject bullet2 = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle));
        GameObject bullet3 = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle+6));

        Bullet bulletScript1 = bullet1.GetComponent<Bullet>();
        Bullet bulletScript2 = bullet2.GetComponent<Bullet>();
        Bullet bulletScript3 = bullet3.GetComponent<Bullet>();
        
        bulletScript1.SetInit(false, "shot_harpoon", stats.GetAttack(), stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), angleMod1 * bulletDir, false);
        bulletScript2.SetInit(false, "shot_harpoon", stats.GetAttack(), stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), bulletDir, false);
        bulletScript3.SetInit(false, "shot_harpoon", stats.GetAttack(), stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), angleMod3 * bulletDir, false);
    }
}
