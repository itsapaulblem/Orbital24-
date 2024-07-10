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
    public float damage = 5f;  // Damage variable
    private float bulletLife = 12f;
    private Vector2 origin;
    private bool randomSplat = true;
    private bool active = true;
    private bool byPlayer = true;
    private bool persist = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        bulletAnimator = GetComponent<Animator>();
        origin = transform.position;
        if (spriteLibrary.spriteLibraryAsset == null)
        {
            // Get default sprite
            spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>(PATHTOSPRITE + "shot_main"); 
        }
    }

    /// Initialise bullet stats
    public void SetInit(bool byPlayer, string shot, float damage, float bulletLife, 
        float velocity, Vector2 dir, bool randomSplat = true)
    {
        this.byPlayer = byPlayer;
        // retrieve bullet sprite
        spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>(PATHTOSPRITE + shot);

        // set bullet stats
        this.damage = damage;
        this.bulletLife = bulletLife;
        this.randomSplat = randomSplat;

        // set bullet velocity
        Rigidbody2D bulletRb = GetComponent<Rigidbody2D>();
        bulletRb.velocity = velocity * dir.normalized;
    }

    // Handle collision on entities
    private void OnTriggerEnter2D(Collider2D collision)
    {  
        if (byPlayer) {
            EnemyAI target = collision.GetComponent<EnemyAI>();
            if (target != null && (active || persist)) {
                target.TakeDamage(damage);
                if (active) { StartCoroutine(End()); }
                active = false;
            }
        } else {
            PlayerController target = collision.GetComponent<PlayerController>();
            if (target != null && (active || persist)) {
                target.TakeDamage(damage);
                if (active) { StartCoroutine(End()); }
                active = false;
            }
        }

        // If the bullet collides with a BoxCollider2D that is a trigger, start the End Coroutine
        BoxCollider2D boxCollider = collision as BoxCollider2D;
        PolygonCollider2D polygonCollider = collision as PolygonCollider2D;
        if ((boxCollider != null && boxCollider.isTrigger && active) || (polygonCollider != null && active)){
            StartCoroutine(End());
            active = false; 
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

    // bullet animation when end
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

    public void Persist() 
    {
        persist = true;
    }
}
