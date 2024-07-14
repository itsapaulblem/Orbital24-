using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB1_AI : EnemyAI
{
    private float lastAttackTime;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Check if undefeated, or if game completed, else destroy
        sight = 20f;
        SetInit(3f, 80f, 5f, 3f);
        audioManager = AudioManager.Instance;
    }

    protected override Vector2 GetSeekingPosition()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (timeSinceLastAttack >= stats.GetAttackSpeed() && state == State.Seeking &&
            Vector2.Distance(player.transform.position, transform.position) <= sight)
        {
            lastAttackTime = Time.time;
            if (audioManager == null) { audioManager = AudioManager.Instance; }
            audioManager.PlaySFX(audioManager.bossOne); // Play hit sound effect
            Quaternion randAngle = Quaternion.Euler(0f,0f, Random.Range(-3f, 3f));
            return randAngle * (player.transform.position - transform.position).normalized * 3;
        } else if (timeSinceLastAttack <= stats.GetAttackSpeed() / 2f && state == State.Seeking) {
            return movement;
        }
        return transform.position-transform.position;
    }
}
