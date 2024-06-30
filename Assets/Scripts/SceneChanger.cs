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
        if (target != null) {
            SceneManager.LoadSceneAsync(nextScene);
            PlayerController.SetCoords(xCoord, yCoord);
        }
    }
}
