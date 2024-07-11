using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingOverlay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer minimapRenderer;
    public void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        minimapRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        if (target != null) {
            StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        if (target != null) {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut(){
        float rate = 1.0f/ 0.5f;
        float progress = 0.0f; 
        Color tmp = spriteRenderer.color;

        while (progress < 1.0f){
            tmp.a = Mathf.Lerp(1, 0.5f , progress);
            spriteRenderer.color = tmp;
            minimapRenderer.color = tmp;
            progress += rate * Time.deltaTime;
            yield return null; 
        }
        tmp.a = 0.5f;
        spriteRenderer.color = tmp;
        minimapRenderer.color = tmp;
    }

    private IEnumerator FadeIn(){
        float rate = 1.0f/ 0.2f;
        float progress = 0.0f; 
        Color tmp = spriteRenderer.color;

        while (progress < 1.0f){
            tmp.a = Mathf.Lerp(0.5f, 1 , progress);
            spriteRenderer.color = tmp;
            minimapRenderer.color = tmp;
            progress += rate * Time.deltaTime;
            yield return null; 
        }
        tmp.a = 1f;
        spriteRenderer.color = tmp;
        minimapRenderer.color = tmp;
    }
}
