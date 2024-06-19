using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    // Combat Attributes
    private string bulletPrefab = "Prefab/Bullet";
    private float lastFireTime;
    // Seeking Attributes
    protected float distanceToStop = 7f;

    void Start()
    {
        SetInit(2f, 30, 10f, 0.9f, 14f, 12f);
        lastFireTime = Time.time - stats.GetAttackSpeed();
    }

    protected override Vector2 GetRoamingPosition()
    {
        return (origin - rb.position).normalized / 2;
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
            return new Vector2(0,0);
        }
    }

    protected override void UpdateEnemyFacingDirection() 
    {
        if (player == null) return;

        if (state == State.Seeking) {
            enemySpriteRenderer.flipX = transform.position.x < player.transform.position.x;
        } else {
            enemySpriteRenderer.flipX = movement.x > 0;
        }
    }

    private void Update() 
    {
        if (player == null) return;

        float timeSinceLastFire = Time.time - lastFireTime;
        if (timeSinceLastFire >= stats.GetAttackSpeed() && state == State.Seeking)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
    }

    private void FireBullet()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 originPos = transform.position;
        Vector2 bulletDir = playerPos - originPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetInit(false, "shot_elec", 
                            stats.GetAttack(), 
                            stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), 
                            bulletDir, 
                            false); // initialise bullet
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(stats.GetAttack() / 2);
        }
    }
}
