using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Bullet : MonoBehaviour
{
    public SpriteLibrary spriteLibrary;
    private Animator bulletAnimator;
    private Vector2 origin;
    private float bulletLife = 200f;
    // Start is called before the first frame update
    void Start()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        bulletAnimator = GetComponent<Animator>();
        origin = transform.position;
        if (spriteLibrary.spriteLibraryAsset == null)
        {
            Debug.Log(Resources.Load<SpriteLibraryAsset>("Sprites/Player/Bullets/shot_main"));
            spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Player/Bullets/shot_main");
        }
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyAI>())
        {
            collision.GetComponent<EnemyAI>();
            StartCoroutine(End());
        }
    }

    private void Update()
    {
        float dist = Vector2.Distance(origin,transform.position);
        if (dist > bulletLife) 
        {
            StartCoroutine(End());
        }
    }

    private IEnumerator End()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        bulletAnimator.SetTrigger("destroy");
        spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Player/Bullets/splat_main");
        yield return new WaitForSeconds(1.5f);
        
        Destroy(gameObject);
    }
}
