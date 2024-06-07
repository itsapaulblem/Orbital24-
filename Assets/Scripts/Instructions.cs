using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class Instructions : MonoBehaviour
{
    public GameObject controls;
    public GameObject scoreText; 
    private CanvasGroup controlsCanvasGroup; 
    public float fadeDuration = 1f; 


    // Start is called before the first frame update
    void Start()
    {
        controlsCanvasGroup = controls.GetComponent<CanvasGroup>();
        if (scoreText != null){
            scoreText.SetActive(false);
        }
       controlsCanvasGroup.alpha = 1;
       controls.SetActive(true);
     }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
           StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade(){
        float startAlpha = controlsCanvasGroup.alpha; 
        float rate = 1.0f/ fadeDuration;
        float progress = 0.0f; 

        while (progress < 1.0f){
            controlsCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0 , progress);
            progress += rate * Time.deltaTime;
            yield return null; 
        }
        controlsCanvasGroup.alpha = 0; 
        controls.SetActive(false);
    }
}
