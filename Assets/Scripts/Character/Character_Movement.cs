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
    private bool canMove = true; // New variable to control movement

    private List<GameObject> interactiveButtons = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();

        // Find all buttons with the ButtonLocation script
        ButtonLocation[] buttonLocations = FindObjectsOfType<ButtonLocation>();
        foreach (var button in buttonLocations)
        {
            interactiveButtons.Add(button.gameObject);
        }
    }

    // Use update for animations
    private void Update()
    {
        // Update the animator
        if(canMove){
            Vector2 movementInput = GetInput();
            UpdateAnimator(movementInput);
            HandleMovement(movementInput);
        }
        else{
            Vector2 movementInput = Vector2.zero;
            HandleMovement(movementInput);
            UpdateAnimator(movementInput);
        }

        CheckForLocationTrigger();
    }

    // New method to toggle player movement
    public void ToggleMovement()
    {
        canMove = !canMove; // Toggle the movement state
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

    void CheckForLocationTrigger()
    {
        Vector2 playerPosition = transform.position;

        // Hide all buttons initially
        foreach (var button in interactiveButtons)
        {
            button.SetActive(false);
        }

        
        // Check if the player's position is close enough to the actual position of each button
        foreach (var button in interactiveButtons)
        {
            ButtonLocation buttonLocation = button.GetComponent<ButtonLocation>();
            if (buttonLocation != null)
            {
                //if(buttonLocation.clicked == true){
                //    ButtonLocation.SetClickedForAllButtons(true);
                //}
                
                Vector2 buttonPosition = buttonLocation.GetButtonPosition();
                float distance = Vector2.Distance(playerPosition, buttonPosition);
                float interactionDistance = buttonLocation.interactionDistance;
                
                // Check if the player is within the interaction distance
                if (distance <= interactionDistance && buttonLocation.clicked == false)
                {
                    button.SetActive(true); // Show the button
                }
            }
        }
    }
}
