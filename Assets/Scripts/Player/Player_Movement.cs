// Rishi Santhanam
// Rohan Ali
// CSCE 482 Siemens Gamification

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    // Add all necessary variables
    // Use SerializeField to expose them in the Unity Editor (Good Code Practice)
    [SerializeField] private float initialSpeed = 1f;
    [SerializeField] private float maxSpeed = 4.5f;
    [SerializeField] private float speedIncreaseDuration = 3f;
    [SerializeField] private float cameraSmoothness = 0.125f;
    [SerializeField] private float cameraZoomIn = 2f;
    [SerializeField] private float cameraZoomOut = 1f;

    private float currentSpeed;
    private float speedIncreaseTimer;

    private bool isMoving = false; // Check if the player is moving
    private bool canMove = true; // New variable to control movement

    private Vector2 lastMovement = Vector2.down;
    
    private Rigidbody2D rb;
    private Animator animator;
    private Camera mainCamera;


    private List<GameObject> interactiveButtons = new List<GameObject>();


    // Start is called before the first frame update (duhhh...)
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        currentSpeed = initialSpeed;

        

        // Find all buttons with the ButtonLocation script
        ButtonLocation[] buttonLocations = FindObjectsOfType<ButtonLocation>();
        foreach (var button in buttonLocations)
        {
            interactiveButtons.Add(button.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleSpeedIncrease();
        CheckForLocationTrigger();
    }

    // FixedUpdate is called at a fixed interval, so it's good for physics
    private void FixedUpdate()
    {
        if(canMove){
            Vector2 movementInput = GetInput();
            HandleMovement(movementInput);
            UpdateAnimator(movementInput);
        }
        else{
            Vector2 movementInput = Vector2.zero;
            HandleMovement(movementInput);
            UpdateAnimator(movementInput);
        }

        if (!isMoving) {
            // Set the current speed to the initial speed
            currentSpeed = initialSpeed; // Stops the robot's recharge
            speedIncreaseTimer = 0f;
        }
    }

    // New method to toggle player movement
    public void ToggleMovement()
    {
        canMove = !canMove; // Toggle the movement state
    }

    // LateUpdate is called after Update, so it's good for camera movement
    private void LateUpdate()
    {
        if(canMove){
            SmoothCameraFollowAndZoom(GetInput());
        }
    }

    // Isn't working right now, but the idea is to increase the speed of the player over time
    // Obviously not necessary, just wanted to try it out
    private void HandleSpeedIncrease()
    {
        if (speedIncreaseTimer < speedIncreaseDuration)
        {
            speedIncreaseTimer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, speedIncreaseTimer / speedIncreaseDuration);
        }
        else
        {
            Debug.Log("Final speed reached: " + currentSpeed);
        }
    }

    // Get the input from the player
    private Vector2 GetInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        return new Vector2(moveHorizontal, moveVertical).normalized;
    }

    // Using rigidbody to move the player
    private void HandleMovement(Vector2 movement)
    {
        // Restrict to horizontal or vertical movement
        if (movement.x != 0)
        {
            movement.y = 0;
        } else if (movement.y != 0)
        {
            movement.x = 0;
        }
        rb.velocity = movement * currentSpeed;
        isMoving = movement != Vector2.zero;
    }

    // Update the animator based on the movement
    // Necessary for the animations to work
    private void UpdateAnimator(Vector2 movement)
    {
        // Restrict to horizontal or vertical movement
        if (movement.x != 0)
        {
            movement.y = 0;
        } else if (movement.y != 0)
        {
            movement.x = 0;
        }
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
        // bool isMoving = movement != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

        if (isMoving) {
            lastMovement = movement;
        }
    }


    // Optional Function, not necessary for the game (Just to mess with camera)
    private void SmoothCameraFollowAndZoom(Vector2 movement)
    {
        // Determine new camera position
        Vector3 desiredPosition = transform.position;
        desiredPosition.z = mainCamera.transform.position.z;
        Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, desiredPosition, cameraSmoothness);
        mainCamera.transform.position = smoothedPosition;

        // Adjust camera zoom based on movement
        float targetZoom = movement != Vector2.zero ? cameraZoomIn : cameraZoomOut;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, cameraSmoothness);
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