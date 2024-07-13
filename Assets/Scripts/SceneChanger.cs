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
                // TODO: Toggle on difficulty selector menu here
                // DifficultyMenuManager should set waiting to be false after selection
                while (waiting) {
                    yield return null;
                }
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
