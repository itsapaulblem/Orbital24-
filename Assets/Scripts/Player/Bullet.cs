using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Bullet : MonoBehaviour
{
    public SpriteLibrary spriteLibrary;
    private Animator bulletAnimator;
    private Vector2 origin;
    private float bulletLife = 12f;
    private bool active = true;
    public int damage = 10;  // Damage variable

    // Start is called before the first frame update
    void Start()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        bulletAnimator = GetComponent<Animator>();
        origin = transform.position;
        if (spriteLibrary.spriteLibraryAsset == null)
        {
            spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Player/Bullets/shot_main");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            StartCoroutine(End());
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
        transform.Rotate(0, 0, Random.Range(0f, 360f));
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
