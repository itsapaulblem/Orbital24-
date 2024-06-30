using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB2_AI : EnemyAI
{
    // Combat Attributes

    private string stompPrefab = "Prefab/Stomp";
    private float lastFireTime;

    private bool contact = false;
   
    void Start()
    {
        // TODO: Check if undefeated, or if game completed, else destroy
        sight = 15f;
        SetInit(4.5f, 150f, 20f, 2f, -1, 0);
        audioManager = AudioManager.Instance;
    }

    private void Update() 
    {
        float timeSinceLastFire = Time.time - lastFireTime;
        if (timeSinceLastFire >= stats.GetAttackSpeed() && state == State.Seeking)
        {
            StompAttack();
            lastFireTime = Time.time;
        }
    }

    private void StompAttack() {
        if (audioManager == null) { audioManager = AudioManager.Instance; }
        audioManager.PlaySFX(audioManager.bossTwo); // Play hit sound effect
        Vector2 bulletDir = transform.position;
        GameObject stomp = Instantiate(Resources.Load(stompPrefab) as GameObject, transform.position + new Vector3(0,-1,0), Quaternion.Euler(0, 0, 0));
        Bullet bulletScript = stomp.GetComponent<Bullet>();
        bulletScript.SetInit(false, "stomp", 
                            stats.GetAttack(), 
                            stats.GetBulletLife(), 
                            stats.GetBulletSpeed(), 
                            bulletDir, 
                            false); // initialise bullet
        bulletScript.Persist();
    }

    protected override void OnCollisionStay2D(Collision2D collision) { 
        contact = true;
    }

    protected void OnCollisionExit2D(Collision2D collision) { 
        contact = false;
    }

    protected override Vector2 GetSeekingPosition()
    {
        if (player == null) return Vector2.zero;

        float dist = Vector2.Distance(player.transform.position, transform.position);
        if (!contact) {
            return (player.transform.position - transform.position).normalized;
        } else {
            return Vector2.zero;
        }
    }
}
