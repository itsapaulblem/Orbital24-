using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float speed = 4f; 
    public float backgroundHeight = 20f; // Height of your background sprite
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; 
    }

    void Update()
    {
        // Move the background downwards
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        
        // If the background moves below the screen, reposition it to create a loop
        if (transform.position.y < -backgroundHeight)
        {
            RepositionBackground();
        }
    }

    private void RepositionBackground()
    {
        // Move the background sprite to the top to create a seamless loop
        Vector3 offset = new Vector3(0, backgroundHeight * 2f, 0);
        transform.position = (Vector3)transform.position + offset;
    }
}
