using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldbossAI : RangedEnemyAI
{   
    private NPC npc;
    private bool triggered = false;
    void Start()
    {
        if (PlayerPrefsManager.isEzekilDead()) Destroy(gameObject);
        // TODO: Check if undefeated, or if game completed, else destroy
        SetInit(2f, 250f, 13f, 1.1f, 15f, 13f); //250hp

        npc = transform.Find("Fieldboss").GetComponent<NPC>();
        if (PlayerPrefsManager.CheckCutscene(2)) {
            npc.dialogueBlock = 1;
            sight = 0f;
            EnemyDied += npc.FieldBossDiedHandler;
        } else {
            triggered = true;
            sight = 16f;
        }
        
        lastFireTime = Time.time - stats.GetAttackSpeed();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (triggered) return;

        IEnumerator AfterDialogue() {
            StartCoroutine(npc.RunText());
            while (!npc.dialogueShown) {
                state = State.Seeking;
                yield return null;
            }
            sight = 16f;
        }

        if (other.CompareTag("Player")) {
            triggered = true;
            StartCoroutine(AfterDialogue());
        }
    }

    protected override Vector2 GetSeekingPosition()
    {
        if (player == null || stats.isDead()) return Vector2.zero;
        if (Mathf.Abs(player.transform.position.y - transform.position.y) < 0.1f) return Vector2.zero;
        return (player.transform.position.y > transform.position.y ? Vector2.up : Vector2.down);
    }

    protected override void UpdateEnemyFacingDirection()
    {
        enemySpriteRenderer.flipX = player.transform.position.x < transform.position.y;
    }


    protected override void FireBullet()
    {
        if (player == null || stats.isDead()) return;
        if (Vector2.Distance(player.transform.position, transform.position) > sight) return;
        if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.bossField); // Play hit sound effect

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

    public override void TakeDamage(float damage)
    {
        if (stats.isDead() || sight == 0) { return; }
        // update healthbar to reflect damage taken
        Vector3 healthBarChange = new Vector3(stats.damage(damage) * maxHealthBarScale, 0.1f, 1f);
        healthBar.transform.localScale = healthBarChange;

        // check if enemy is dead
        if (stats.isDead())
        {
            audioManager.PlaySFX(audioManager.enemyDied); // Play death sound effect
            // Notify listeners that the enemy has died
            IEnumerator AfterDialogue() {
                if (!PlayerPrefsManager.isComplete()) {
                    TriggerListener();
                    while (!npc.dialogueShown) {
                        yield return null;
                    }
                }
                SpriteRenderer bsr = GetComponent<SpriteRenderer>();
                float rate = 1.0f/ 0.5f;
                float progress = 0.0f; 
                Color tmp = bsr.color;

                while (progress < 2.0f){
                    tmp.a = Mathf.Lerp(1, 0 , progress);
                    bsr.color = tmp;
                    progress += rate * Time.deltaTime;
                    yield return null; 
                }
                Destroy(gameObject);
                GameManager.Instance.AddKill();
            }
            StartCoroutine(AfterDialogue());
        } else {    
            // If enemy not dead
            StartCoroutine(FlashEffect());
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.enemybeingshot); // Play hit sound effect
        }
    }
}
