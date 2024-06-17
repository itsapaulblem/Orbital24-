using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject confirmMenu; 
    void Start()
    {
        confirmMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClick(){
        confirmMenu.SetActive(true);
    }
}
