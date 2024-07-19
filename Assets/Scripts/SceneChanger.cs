using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private float xCoord;
    [SerializeField] private float yCoord;
    [SerializeField] public bool difficultySelector = false;
    public bool waiting = true;

    public void Start()
    {
        PlayableDirector director = gameObject.GetComponent<PlayableDirector>();
        if (director != null) {
            StartCoroutine(CheckEnd(director));
        }

        if (difficultySelector && !PlayerPrefsManager.isComplete()) {
            difficultySelector = false;
        }
    }

    public IEnumerator CheckEnd(PlayableDirector director)  
    {
        while (director.state == PlayState.Playing) 
        {  yield return new WaitForEndOfFrame();  }   
        SceneManager.LoadSceneAsync(nextScene);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        PlayerController target = collision.GetComponent<PlayerController>();

        IEnumerator ChangeSceneWhenReady() {
            if (difficultySelector) {
                GameManager.Instance.ToggleDifficultyMenu();
                // DifficultyMenuManager should set waiting to be false after selection
                while (waiting) {
                    yield return null;
                }
            }

            // FadeOut
            GameObject fade = GameObject.Find("Main Camera").transform.Find("Fade").gameObject;
            SpriteRenderer fsr = fade.GetComponent<SpriteRenderer>();
            float rate = 1f/ 2f;
            float progress = 0.0f; 
            Color tmp = fsr.color;
            fade.SetActive(true);

            while (progress < 1.0f){
                tmp.a = Mathf.Lerp(0, 1 , progress);
                fsr.color = tmp;
                progress += rate * Time.deltaTime;
                yield return null; 
            }

            GameManager.Instance.lastScene = (nextScene == "Dungeon" ? "Room" : nextScene);
            PlayerPrefsManager.SetLastScene(nextScene == "Dungeon" ? "Room" : nextScene);
            PlayerController.SetCoords(xCoord, yCoord);
            SceneManager.LoadSceneAsync(nextScene);
        }
        if (target != null) {
            StartCoroutine(ChangeSceneWhenReady());
        }
    }
}
