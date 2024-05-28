using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 4f;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRenderer;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() 
    {
        
    }

    private void FixedUpdate() 
    {
        UpdatePlayerFacingDirection();
        Move();
    }

    private void OnMove(InputValue inputValue) 
    {
        movement = inputValue.Get<Vector2>();
        movement.Normalize();
        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetFloat("moveY", movement.y);
    }

    private void Move() 
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private void UpdatePlayerFacingDirection() 
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 cameraPoint = Camera.main.WorldToScreenPoint(transform.position);
        playerSpriteRenderer.flipX = mousePos.x < cameraPoint.x;
    }
}
