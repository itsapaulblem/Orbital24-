using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBehavior : MonoBehaviour
{
    [SerializeField] private GameObject[] linkedObject;
    private Dictionary<GameObject, Vector3> origins = new Dictionary<GameObject, Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in linkedObject) {
            origins.Add(obj, obj.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.transform.Find("Activated").gameObject.SetActive(true);
        PlayerController target = collision.GetComponent<PlayerController>();
        if (target != null) {
            foreach (GameObject obj in linkedObject) {
                StartCoroutine(Reset(obj));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.transform.Find("Activated").gameObject.SetActive(false);
    }

    private IEnumerator Reset(GameObject obj) 
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 origin = origins[obj];
        Vector2 movement = (origin - rb.position);
        float timeStarted = Time.time;
        while (Vector2.Distance(origin, obj.transform.position) > 0.5f) {
            movement = (origin - rb.position).normalized;
            rb.MovePosition(rb.position + movement * 10 * Time.fixedDeltaTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            if (Time.time - timeStarted > 5f) {
                // if object is stuck
                break;
            }
            rb.MovePosition(origin);
        }
        
        
    }
}
