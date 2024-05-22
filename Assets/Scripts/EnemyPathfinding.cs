using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 50f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer enemySpriteRenderer;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);   
        UpdateEnemyFacingDirection();
    }

    public void MoveTo(Vector2 targetPos)
    {
        movement = targetPos;
    }

    private void UpdateEnemyFacingDirection() 
    {
        enemySpriteRenderer.flipX = movement.x > 0;
    }
}
