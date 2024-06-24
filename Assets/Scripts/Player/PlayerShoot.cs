using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    // Bullet prefab to be instantiated when shooting 
    [SerializeField] private GameObject bulletPrefab;
    private float bulletSpeed = 120f;
    // Flags to determine shooting behaviour 
    private bool shootContinuous;
    private bool shootSingle;
    private float timeBetweenShots = 0.9f;
    // Timestamp of the last shot fired 
    private float lastFireTime;
    

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
    // Method to instantiate and fire a bullet 
    private void FireBullet()
    {
         // Get the mouse position in world coordinates.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate the direction from the player to the mouse position.
        Vector2 bulletDir = mousePos - transform.position;
        
        // Calculate the angle to rotate the bullet towards the mouse position.
        float bulletAngle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;

        // Instantiate the bullet at the player's position, rotated towards the mouse position.
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, bulletAngle));
        
        // Get the Rigidbody2D component of the bullet to set its velocity.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
        // Set the bullet's velocity in the calculated direction.
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
