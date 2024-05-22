using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private float bulletSpeed = 40f;
    private bool shootContinuous;
    private bool shootSingle;
    private float timeBetweenShots = 0.9f;
    private float lastFireTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (shootContinuous || shootSingle)
        {
            float timeSinceLastFire = Time.time - lastFireTime;
            
            if (timeSinceLastFire >= timeBetweenShots)
            {
                FireBullet();
                lastFireTime = Time.time;
                shootSingle = false;
            }
        }
    }

    private void FireBullet()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bulletDir = mousePos - transform.position;
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0 , bulletAngle));
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bullet.GetComponent<Animator>().Rebind();
        bullet.GetComponent<SpriteLibrary>().spriteLibraryAsset = Resources.Load("Sprites/Player/Bullets/shot_main") as SpriteLibraryAsset;

        bulletRb.velocity = bulletSpeed * bulletDir.normalized;
    }

    private void OnFire(InputValue inputValue)
    {
        shootContinuous = inputValue.isPressed;
        if (inputValue.isPressed)
        {
            shootSingle = true;
        }
    }
}
