using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLayering : MonoBehaviour
{
    private SpriteRenderer obstacleRenderer;
    private GameObject player;
    
    int currentSortOrder = 1;   // default to all obstacle in front

    const int IN_FRONT = 1;
    const int IN_BACK = -1;


    void Awake() {
        obstacleRenderer = GetComponent<SpriteRenderer>();
        player = ObstacleLayeringManager.Instance.Player;
        SetSortOrder();
    }


    void SetSortOrder() {
        obstacleRenderer.sortingOrder = currentSortOrder;
    }


    void Update() {
        int order = (player.transform.position.y < transform.position.y) ? IN_BACK : IN_FRONT;
        if (order != currentSortOrder)
        {
            currentSortOrder = order;
            SetSortOrder();
        }
    }
}
