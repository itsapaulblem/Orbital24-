using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordCutscene : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController target = collision.GetComponent<PlayerController>();
        // If the collider is a player, proceed to collect the item 
        if (target != null) {
            CoroutineManager.Instance.StartCoroutine(StartCutscene2());
            PlayerPrefsManager.Complete();
            Destroy(gameObject);
        }
    }

    IEnumerator StartCutscene2() {
        // Fade out
        GameObject fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
        SpriteRenderer fsr = fade.GetComponent<SpriteRenderer>();
        float faderate = 1f/ 2f;
        float fadeprogress = 0.0f; 
        Color tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fade.SetActive(true);

        while (fadeprogress < 1.0f){
            tmp.a = Mathf.Lerp(0, 1 , fadeprogress);
            fsr.color = tmp;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }
        // Transit to Seagrass valley
        PlayerController.SetCoords(-32.6f, 133.6f);
        var waitAsycn = SceneManager.LoadSceneAsync("SeagrassValley");
        yield return new WaitUntil(() => waitAsycn.isDone);
        yield return new WaitForSeconds(0.3f);

        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().canMove = false;
        player.GetComponent<SpriteRenderer>().flipX = false;
        player.GetComponent<Animator>().SetFloat("moveX", 1);
        float rate = 1f/ 6f;
        float progress = 0.0f; 
        fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
        fsr = fade.GetComponent<SpriteRenderer>();
        tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fadeprogress = 0f;

        while (progress < 0.7f) {
            player.transform.position = new Vector2(Mathf.Lerp(-32.6f, -29.6f, progress), Mathf.Lerp(133.6f, 123.6f, progress));
            progress += rate * Time.deltaTime;
            yield return null; 
        }
        // Fade out
        fade.SetActive(true);
        while (fadeprogress < 1.0f){
            player.transform.position = new Vector2(Mathf.Lerp(-32.6f, -29.6f, progress), Mathf.Lerp(133.6f, 123.6f, progress));
            tmp.a = Mathf.Lerp(0, 1 , fadeprogress);
            fsr.color = tmp;
            progress += rate * Time.deltaTime;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }
        // Transit to Town
        PlayerController.SetCoords(15f, 3.5f);
        waitAsycn = SceneManager.LoadSceneAsync("Town");
        yield return new WaitUntil(() => waitAsycn.isDone);
        yield return new WaitForSeconds(0.3f);

        player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().canMove = false;
        player.GetComponent<SpriteRenderer>().flipX = false;
        player.GetComponent<Animator>().SetFloat("moveX", 1);
        rate = 1f/ 7f;
        progress = 0.0f; 
        fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
        fsr = fade.GetComponent<SpriteRenderer>();
        tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fadeprogress = 0f;

        while (progress < 0.7f) {
            player.transform.position = new Vector2(Mathf.Lerp(15, 25 , progress), 3.5f);
            progress += rate * Time.deltaTime;
            yield return null;
        }
        // Fade out
        fade.SetActive(true);
        while (fadeprogress < 1.0f){
            player.transform.position = new Vector2(Mathf.Lerp(15, 25 , progress), 3.5f);
            tmp.a = Mathf.Lerp(0, 1 , fadeprogress);
            fsr.color = tmp;
            progress += rate * Time.deltaTime;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }
        // Transit to Room
        PlayerController.SetCoords(0f, -1.3f);
        waitAsycn = SceneManager.LoadSceneAsync("Room");
        yield return new WaitUntil(() => waitAsycn.isDone);
        yield return new WaitForSeconds(0.3f);

        player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().canMove = false;
        player.GetComponent<SpriteRenderer>().flipX = false;
        player.GetComponent<Animator>().SetFloat("moveX", 1);
        rate = 1f/ 0.8f;
        progress = 0.0f; 
        while (progress < 1.0f) {
            player.transform.position = new Vector2(0f, Mathf.Lerp(-1.3f, -0.6f , progress));
            progress += rate * Time.deltaTime;
            yield return null;
        }
        rate = 1f/ 3.2f;
        progress = 0.0f; 
        while (progress < 1.0f) {
            player.transform.position = new Vector2(Mathf.Lerp(0f, 2.75f , progress), -0.6f);
            progress += rate * Time.deltaTime;
            yield return null;
        }
        player.GetComponent<Animator>().SetFloat("moveX", 0);
        yield return new WaitForSeconds(1f);
        GameObject blankSwordPrefab = Resources.Load<GameObject>("Prefab/BlankSword");
        GameObject sword = Instantiate(blankSwordPrefab, new Vector3(3.8f, -0.4f, 0), Quaternion.identity); 
        SpriteRenderer ssr = sword.GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(7.2f);
        AudioManager am = AudioManager.Instance;
        for (int _ = 1; _ <= 5; _++) {
            yield return new WaitForSeconds(0.8f);
            am.PlaySFX(am.munch);
            ssr.sprite = Resources.Load<Sprite>("Sprites/QuestObject/spr_bite_" + _);
        }
        // Fade out
        fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
        fsr = fade.GetComponent<SpriteRenderer>();
        tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fadeprogress = 0.0f;
        fade.SetActive(true);

        while (fadeprogress < 1.0f){
            tmp.a = Mathf.Lerp(0, 1 , fadeprogress);
            fsr.color = tmp;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }

        // Thankyou Fade in/out
        fade = GameObject.Find("Main Camera").transform.Find("Thankyou").gameObject;
        fsr = fade.GetComponent<SpriteRenderer>();
        tmp = fsr.color;
        tmp.a = 0f;
        fsr.color = tmp;
        fadeprogress = 0.0f;
        fade.SetActive(true);

        while (fadeprogress < 1.0f){
            tmp.a = Mathf.Lerp(0, 1 , fadeprogress);
            fsr.color = tmp;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }
        yield return new WaitForSeconds(3f);
        fadeprogress = 0;
        while (fadeprogress < 1.0f){
            tmp.a = Mathf.Lerp(1, 0 , fadeprogress);
            fsr.color = tmp;
            fadeprogress += faderate * Time.deltaTime;
            yield return null; 
        }
        SceneManager.LoadSceneAsync("Room");
    }

    
}
