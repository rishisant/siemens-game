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
    private Animator child_ChestAnimator;

    private bool syncFlag = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Use update for animations
    private void Update()
    {
        // Update the animator
        Vector2 movementInput = GetInput();
        UpdateAnimator(movementInput);
        HandleMovement(movementInput);
        Debug.Log("Parent: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Debug.Log("Child: " + child_ChestAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

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

        // Update the child animator (Chest)
        child_ChestAnimator.SetFloat("MovingX", movement.x);
        child_ChestAnimator.SetFloat("MovingY", movement.y);
        child_ChestAnimator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            lastMovement = movement;
        }
    }
}
