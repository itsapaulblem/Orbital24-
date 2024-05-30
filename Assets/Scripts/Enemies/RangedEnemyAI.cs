using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    // Combat Attributes
    private string bulletPrefab = "Prefab/Bullet";
    float bulletLife = 14f;
    float bulletSpeed = 12f;
    private float timeBetweenShots = 0.9f;
    private float lastFireTime;
    // Seeking Attributes
    private float distanceToStop = 7f;

    void Start()
    {
        SetInit(30, 10f, 2f);
        lastFireTime = Time.time - timeBetweenShots;
    }

    protected override Vector2 GetRoamingPosition()
    {
        return (origin - rb.position).normalized / 2;
    }

    protected override Vector2 GetSeekingPosition()
    {
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
        if (state == State.Seeking) {
            enemySpriteRenderer.flipX = transform.position.x < player.transform.position.x;
        } else {
            enemySpriteRenderer.flipX = movement.x > 0;
        }
    }

    private void Update() 
    {
        float timeSinceLastFire = Time.time - lastFireTime;
        float dist = Vector2.Distance(player.transform.position, transform.position);
        if (timeSinceLastFire >= timeBetweenShots && state == State.Seeking)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
    }

    private void FireBullet()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 originPos = transform.position;
        Vector2 bulletDir = playerPos - originPos;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(Resources.Load(bulletPrefab) as GameObject, transform.position, Quaternion.Euler(0, 0, bulletAngle));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetInit(false, "shot_elec", attack, bulletLife, bulletSpeed, bulletDir); // initialise bullet
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(attack/2);
        }
    }
    
}
