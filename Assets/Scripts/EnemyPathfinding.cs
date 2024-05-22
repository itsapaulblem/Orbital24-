using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 50f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);   
    }

    public void MoveTo(Vector2 targetPos)
    {
        movement = targetPos;
    }
    
}
