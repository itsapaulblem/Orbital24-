using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public GameObject instructionsPage;

    // Start is called before the first frame update
    void Start()
    {
        instructionsPage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)){
            instructionsPage.SetActive(false);
        }
    }
}
