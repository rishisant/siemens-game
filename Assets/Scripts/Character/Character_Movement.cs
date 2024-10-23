// Rishi Santhanam
// CSCE 482 Siemens Gamification

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character_Movement : MonoBehaviour
{
	[SerializeField] private float charSpeed = 4f;
	[SerializeField] private CosmeticHandler cosmeticHandler;

	private string currentState;

	private Rigidbody2D rb;
	private Animator animator;
	private Animator child_ChestAnimator;
	private Animator child_LegAnimator;
	private Animator child_ShoeAnimator;
	private Animator child_HatAnimator;

	// Grab the equipped items dictionary from PlayerData
	[SerializeField] private PlayerData playerData;
	private List<int> equipped_items => playerData.equipped_items;
	private List<int> original_load_items = new List<int> { 100, 200, 300, 400 };

	private Vector2 movementInputDirection;
	private Vector2 lastMovementInputDirection;

	private Dictionary<Vector2, string> playerMovingAnimations;
	private Dictionary<Vector2, string> playerIdleAnimations;

	private Dictionary<Vector2, string> chestMovingAnimations;
	private Dictionary<Vector2, string> chestIdleAnimations;

	private Dictionary<Vector2, string> legMovingAnimations;
	private Dictionary<Vector2, string> legIdleAnimations;

	private Dictionary<Vector2, string> hatMovingAnimations;
	private Dictionary<Vector2, string> hatIdleAnimations;

	private Dictionary<Vector2, string> shoeMovingAnimations;
	private Dictionary<Vector2, string> shoeIdleAnimations;

	private bool syncFlag = false;
    private bool canMove = true; // New variable to control movement

    private List<GameObject> interactiveButtons = new List<GameObject>();
	private bool isResettingAnimations;
	private bool isEmoting;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();
		child_LegAnimator = transform.GetChild(1).GetComponent<Animator>();
		child_ShoeAnimator = transform.GetChild(2).GetComponent<Animator>();
		child_HatAnimator = transform.GetChild(3).GetComponent<Animator>();

		CreateAnimationDictionary();

		lastMovementInputDirection = Vector2.down;

		// Set the chestsprite to the one in the equipped items
		// Organize equippedItems by lowest to highest id
		equipped_items.Sort();

		SetChestSprite(cosmeticHandler.GetChestController(equipped_items[1] - 200));
		SetLegSprite(cosmeticHandler.GetLegController(equipped_items[2] - 300));
		SetShoeSprite(cosmeticHandler.GetShoeController(equipped_items[3] - 400));
		SetHatSprite(cosmeticHandler.GetHatController(equipped_items[0] - 100));

		// Change the original_load_items to the equipped items
		original_load_items = equipped_items;

        // Find all buttons with the ButtonLocation script
        ButtonLocation[] buttonLocations = FindObjectsOfType<ButtonLocation>();
        foreach (var button in buttonLocations)
        {
            interactiveButtons.Add(button.gameObject);
        }
	}

	// Can't move function
	public void StopPlayer()
	{
		// Stop the player from moving
		canMove = false;

		// Change movement input direction to zero
		movementInputDirection = Vector2.zero;

		// Update player animations
		UpdateAnimator();
	}

	// Can move function
	public void UnstopPlayer()
	{
		// Allow the player to move
		canMove = true;
	}

	// Use update for animations
	private void Update()
	{
		if (original_load_items != equipped_items)
		{
			// Organize equippedItems by lowest to highest id
			equipped_items.Sort();

			SetChestSprite(cosmeticHandler.GetChestController(equipped_items[1] - 200));
			SetLegSprite(cosmeticHandler.GetLegController(equipped_items[2] - 300));
			SetShoeSprite(cosmeticHandler.GetShoeController(equipped_items[3] - 400));
			SetHatSprite(cosmeticHandler.GetHatController(equipped_items[0] - 100));

			// Change the original_load_items to the equipped items
			original_load_items = equipped_items;
		}


		// Update the animator
		movementInputDirection = GetInput();
        if(canMove)
		    UpdateAnimator();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			StopAllCoroutines();
			isEmoting = false;

			SetChestSprite(cosmeticHandler.GetChestController(Random.Range(0, cosmeticHandler.ChestAnimControllerLenght())));
			SetLegSprite(cosmeticHandler.GetLegController(Random.Range(0, cosmeticHandler.LegAnimControllerLenght())));
			SetShoeSprite(cosmeticHandler.GetShoeController(Random.Range(0, cosmeticHandler.ShoeControllerLenght())));
			SetHatSprite(cosmeticHandler.GetHatController(Random.Range(0, cosmeticHandler.HatAnimControllerLenght())));
		}

		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
		{
			StopAllCoroutines();
			PerformEmote(1);
		}
		if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
		{
			StopAllCoroutines();
			PerformEmote(2);
		}
		if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
		{
			StopAllCoroutines();
			PerformEmote(3);
		}
	}

	private void FixedUpdate()
	{
		if(canMove)
            HandleMovement(movementInputDirection);
        else{
            Vector2 movementInput = Vector2.zero;
            HandleMovement(movementInput);
            isEmoting = false;
        }
        CheckForLocationTrigger();
	}

    // New method to toggle player movement
    public void ToggleMovement()
    {
        canMove = !canMove; // Toggle the movement state
    }

	private void ChangeAnimationState(Animator animator, string newState, ref string currentState)
	{
		if (currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}

	private void ChangePlayerAnimationState(string newState)
	{
		ChangeAnimationState(animator, newState, ref currentState);
	}

	private void ChangeChestAnimationState(string newState)
	{
		ChangeAnimationState(child_ChestAnimator, newState, ref currentState);
	}

	private void ChangeLegAnimationState(string newState)
	{
		ChangeAnimationState(child_LegAnimator, newState, ref currentState);
	}

	private void ChangeShoeAnimationState(string newState)
	{
		ChangeAnimationState(child_ShoeAnimator, newState, ref currentState);
	}
	private void ChangeHatAnimationState(string newState)
	{
		ChangeAnimationState(child_HatAnimator, newState, ref currentState);
	}

	private Vector2 GetInput()
	{
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");
		return new Vector2(moveHorizontal, moveVertical).normalized;
	}

	private void HandleMovement(Vector2 movement)
	{
		// Movement restriction
		if (movement.x != 0)
		{
			movement.y = 0;
		}
		else if (movement.y != 0)
		{
			movement.x = 0;
		}

		if (movement != Vector2.zero)
		{
			// Stops emoting

			StopAllCoroutines();
			isEmoting = false;
		}

		rb.velocity = movement * charSpeed;
	}

	private void UpdateAnimator()
	{
		if (isResettingAnimations || isEmoting) return;

		if (movementInputDirection != Vector2.zero)
		{
			if (playerMovingAnimations.ContainsKey(movementInputDirection))
			{
				ChangePlayerAnimationState(playerMovingAnimations[movementInputDirection]);
				ChangeChestAnimationState(chestMovingAnimations[movementInputDirection]);
				ChangeLegAnimationState(legMovingAnimations[movementInputDirection]);
				ChangeShoeAnimationState(shoeMovingAnimations[movementInputDirection]);
				ChangeHatAnimationState(hatMovingAnimations[movementInputDirection]);

				lastMovementInputDirection = movementInputDirection;
			}
		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				ChangePlayerAnimationState(playerIdleAnimations[lastMovementInputDirection]);
				ChangeChestAnimationState(chestIdleAnimations[lastMovementInputDirection]);
				ChangeLegAnimationState(legIdleAnimations[lastMovementInputDirection]);
				ChangeShoeAnimationState(shoeIdleAnimations[lastMovementInputDirection]);
				ChangeHatAnimationState(hatIdleAnimations[lastMovementInputDirection]);
			}
		}
	}

	// Set the chest sprite based on the prefab from the CosmeticHandler
	public void SetChestSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_ChestAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetLegSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_LegAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetShoeSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_ShoeAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetHatSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_HatAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}

	private void PerformEmote(int emoteIndex)
	{
		isEmoting = true;

		// Play the emote animation
		ChangePlayerAnimationState("Char_Emote_" + emoteIndex);
		ChangeChestAnimationState("Chest_Emote_" + emoteIndex);
		ChangeLegAnimationState("Leg_Emote_" + emoteIndex);
		ChangeShoeAnimationState("Shoe_Emote_" + emoteIndex);
		ChangeHatAnimationState("Hat_Emote_" + emoteIndex);

		// Start a coroutine to wait for the emote animation to finish
		StartCoroutine(WaitForEmoteToFinish(emoteIndex));
	}

	private IEnumerator WaitForEmoteToFinish(int emoteIndex)
	{
		// Wait until the emote animation is fully playing
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Char_Emote_" + emoteIndex))
		{
			yield return null; // Wait for the next frame
		}

		// Get the length of the emote animation
		float emoteLength = animator.GetCurrentAnimatorStateInfo(0).length;

		// Wait for the emote animation to finish
		yield return new WaitForSeconds(emoteLength);

		// After emote finishes, stop emoting and return to idle
		StopEmoting();
	}

	private void StopEmoting()
	{
		isEmoting = false;

		ChangePlayerAnimationState("Char_Idle_Down");
		ChangeChestAnimationState("Chest_Idle_Down");
		ChangeLegAnimationState("Leg_Emote_Idle");
		ChangeShoeAnimationState("Shoe_Emote_Idle");
		ChangeHatAnimationState("Hat_Emote_Idle");
	}

	private void SyncAnimations(Vector2 movement)
	{
		isResettingAnimations = true;

		// Movement restriction
		if (movement.x != 0)
		{
			movement.y = 0;

			if (movement.x > 0)
			{
				movement.x = 1;
			}
			else if (movement.x < 0)
			{
				movement.x = -1;
			}
		}
		else if (movement.y != 0)
		{
			movement.x = 0;

			if (movement.y > 0)
			{
				movement.y = 1;
			}
			else if (movement.y < 0)
			{
				movement.y = -1;
			}
		}

		if (movement != Vector2.zero)
		{
			if (playerMovingAnimations.ContainsKey(movement))
			{
				animator.Play(playerMovingAnimations[movement], 0, 0f);
				child_ChestAnimator.Play(chestMovingAnimations[movement], 0, 0f);
				child_LegAnimator.Play(legMovingAnimations[movement], 0, 0f);
				child_ShoeAnimator.Play(shoeMovingAnimations[movement], 0, 0f);
				child_HatAnimator.Play(hatMovingAnimations[movement], 0, 0f);

				lastMovementInputDirection = movement;
			}

		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				animator.Play(playerIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_ChestAnimator.Play(chestIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_LegAnimator.Play(legIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_ShoeAnimator.Play(shoeIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_HatAnimator.Play(hatIdleAnimations[lastMovementInputDirection], 0, 0f);
			}
		}


		isResettingAnimations = false;
	}

	private void CreateAnimationDictionary()
	{
		playerMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Char_Walk_Down" },
			{ Vector2.up, "Char_Walk_Up" },
			{ Vector2.left, "Char_Walk_Left" },
			{ Vector2.right, "Char_Walk_Right" }
		};

		playerIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Char_Idle_Down" },
			{ Vector2.up, "Char_Idle_Up" },
			{ Vector2.left, "Char_Idle_Left" },
			{ Vector2.right, "Char_Idle_Right" }
		};

		chestMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Chest_Walk_Down" },
			{ Vector2.up, "Chest_Walk_Up" },
			{ Vector2.left, "Chest_Walk_Left" },
			{ Vector2.right, "Chest_Walk_Right" }
		};

		chestIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Chest_Idle_Down" },
			{ Vector2.up, "Chest_Idle_Up" },
			{ Vector2.left, "Chest_Idle_Left" },
			{ Vector2.right, "Chest_Idle_Right" }
		};

		legMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Leg_Walk_Down" },
			{ Vector2.up, "Leg_Walk_Up" },
			{ Vector2.left, "Leg_Walk_Left" },
			{ Vector2.right, "Leg_Walk_Right" }
		};

		legIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Leg_Idle_Down" },
			{ Vector2.up, "Leg_Idle_Up" },
			{ Vector2.left, "Leg_Idle_Left" },
			{ Vector2.right, "Leg_Idle_Right" }
		};

		shoeMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Shoe_Walk_Down" },
			{ Vector2.up, "Shoe_Walk_Up" },
			{ Vector2.left, "Shoe_Walk_Left" },
			{ Vector2.right, "Shoe_Walk_Right" }
		};

		shoeIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Shoe_Idle_Down" },
			{ Vector2.up, "Shoe_Idle_Up" },
			{ Vector2.left, "Shoe_Idle_Left" },
			{ Vector2.right, "Shoe_Idle_Right" }
		};

		hatMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Hat_Walk_Down" },
			{ Vector2.up, "Hat_Walk_Up" },
			{ Vector2.left, "Hat_Walk_Left" },
			{ Vector2.right, "Hat_Walk_Right" }
		};

		hatIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Hat_Idle_Down" },
			{ Vector2.up, "Hat_Idle_Up" },
			{ Vector2.left, "Hat_Idle_Left" },
			{ Vector2.right, "Hat_Idle_Right" }
		};
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
