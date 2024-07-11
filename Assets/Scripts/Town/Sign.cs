using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    public GameObject words;

    void Start() {
        if (words == null) {
            words = gameObject.transform.GetChild(0).gameObject;
        }
        words.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (words != null) words.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (words != null) words.SetActive(false);
        }
    }
}
