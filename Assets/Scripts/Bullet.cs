using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Bullet : MonoBehaviour
{
    // Animation Attribute
    public SpriteLibrary spriteLibrary;
    private Animator bulletAnimator;
    private string PATHTOSPRITE = "Sprites/Bullets/";
    // Bullet Attribute
    public float damage = 10f;  // Damage variable
    private float bulletLife = 12f;
    private Vector2 origin;
    private bool randomSplat = true;
    private bool active = true;
    private bool byPlayer = true;
    

    // Start is called before the first frame update
    void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        bulletAnimator = GetComponent<Animator>();
        origin = transform.position;
        if (spriteLibrary.spriteLibraryAsset == null)
        {
            spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>(PATHTOSPRITE + "shot_main"); // default
        }
    }

    public void SetInit(bool byPlayer, string shot, float damage, float bulletLife, float velocity, Vector2 dir, bool randomSplat = true)
    {
        this.byPlayer = byPlayer;
        spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>(PATHTOSPRITE + shot);
        this.damage = damage;
        this.bulletLife = bulletLife;
        this.randomSplat = randomSplat;

        Rigidbody2D bulletRb = GetComponent<Rigidbody2D>();
        bulletRb.velocity = velocity * dir.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (byPlayer) {
            EnemyAI target = collision.GetComponent<EnemyAI>();
            if (target != null && active) {
                active = false;
                target.TakeDamage(damage);
                StartCoroutine(End());
            }
        } else {
            PlayerController target = collision.GetComponent<PlayerController>();
            if (target != null && active) {
                active = false;
                target.TakeDamage(damage);
                StartCoroutine(End());
            }
        }
        
    }

    private void Update()
    {
        float dist = Vector2.Distance(origin, transform.position);
        if (dist > bulletLife && active)
        {
            StartCoroutine(End());
            active = false;
        }
    }

    private IEnumerator End()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        bulletAnimator.SetTrigger("destroy");
        if (randomSplat) {
            transform.Rotate(0, 0, Random.Range(0f, 360f));
        }
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
