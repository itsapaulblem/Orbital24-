using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(CheckEnd());
    }

    public IEnumerator CheckEnd()  
    {
        PlayableDirector director = gameObject.GetComponent<PlayableDirector>();
        while (director.state == PlayState.Playing) 
        {  yield return new WaitForEndOfFrame();  }   
        OnEnd();   
    }  

    public void OnEnd()   
    {  
        Debug.Log("On end"); 
        SceneManager.LoadSceneAsync("Town");
    }
}
