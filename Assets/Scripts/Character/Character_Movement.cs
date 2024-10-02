// Rishi Santhanam
// CSCE 482 Siemens Gamification

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    [SerializeField] private float charSpeed = 4f;

    private bool isMoving = false;
    private Vector2 lastMovement = Vector2.down;
    
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 movementInput = GetInput();
        HandleMovement(movementInput);
        UpdateAnimator(movementInput);
    }

    private Vector2 GetInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        return new Vector2(moveHorizontal, moveVertical).normalized;
    }

    private void HandleMovement(Vector2 movement)
    {
        if (movement.x != 0)
        {
            movement.y = 0;
        }
        else if (movement.y != 0)
        {
            movement.x = 0;
        }

        rb.velocity = movement * charSpeed; // Use charSpeed
        isMoving = movement != Vector2.zero;
    }

    private void UpdateAnimator(Vector2 movement)
    {
        animator.SetFloat("MovingX", movement.x);
        animator.SetFloat("MovingY", movement.y);
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            lastMovement = movement;
        }
    }
}
