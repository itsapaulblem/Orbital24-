using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class Instructions : MonoBehaviour
{
    public GameObject instructionsPage;
    public GameObject scoreText; 

    // Start is called before the first frame update
    void Start()
    {
        instructionsPage.SetActive(true);
        scoreText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)){
            instructionsPage.SetActive(false);
            scoreText.SetActive(true);
        }
    }
}
