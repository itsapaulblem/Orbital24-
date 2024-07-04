using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
public class Instructions : MonoBehaviour
{
    public GameObject controls;
    private CanvasGroup controlsCanvasGroup; 
    public float fadeDuration = 1f; 
    private bool faded = false;


    // Start is called before the first frame update
    void Start()
    {
        controlsCanvasGroup = controls.GetComponent<CanvasGroup>();
        controlsCanvasGroup.alpha = 1;
        if (PlayerPrefsManager.CheckCutscene(1)) {
            controls.SetActive(true);
            faded = false;
        } else {
            controls.SetActive(false);
            faded = true;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (controls != null && controlsCanvasGroup == null) { controlsCanvasGroup = controls.GetComponent<CanvasGroup>(); }
        if (controlsCanvasGroup == null) { return; }

        if (!faded && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) {
            faded = true;
            StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade(){
        if (controlsCanvasGroup == null) { yield break; }
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

        // TODO: Set increment to disable instructions
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Room") {
            if (controls == null) { controls = GameObject.Find("Controls"); }

            controlsCanvasGroup = controls.GetComponent<CanvasGroup>();
            controlsCanvasGroup.alpha = 1;
            if (PlayerPrefsManager.CheckCutscene(1)) {
                controls.SetActive(true);
                faded = false;
            } else {
                controls.SetActive(false);
                faded = true;
            }
            Debug.Log(faded);
        }
    }
}
