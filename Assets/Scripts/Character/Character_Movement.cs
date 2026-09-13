// Rishi Santhanam
// Byte City character controls

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * @brief This class handles character movement. This script will be attached
 * to the player object
 *
 * There are lots of moving parts, since we have separate entities for the
 * player's head, chest, arms, legs, as well as handling movement for all of
 * their cosmetics. Emotes are also handled in this class.
 *
 * @see CosmeticHandler
 */
public class Character_Movement : MonoBehaviour
{
	//charSpeed is gotten from playerData.movement_speed
	[SerializeField] private CosmeticHandler cosmeticHandler;
	private float charSpeed => PlayerData.Instance.movement_speed;

	private string currentState;
    private string chestState, legState, shoeState, hatState;
    private Vector2 touchInput;
    private readonly SprintState sprint = new SprintState();
    public bool IsSprinting { get { return sprint.IsSprinting; } }
    private Coroutine emoteRoutine;
    public bool CanMove { get { return canMove && !WorldUiFocus.Blocked; } }
    public bool NetworkMoving { get { return rb != null && rb.velocity.sqrMagnitude > 0.01f; } }
    public string NetworkFacing { get { return lastMovementInputDirection == Vector2.up ? "Up" : lastMovementInputDirection == Vector2.left ? "Left" : lastMovementInputDirection == Vector2.right ? "Right" : "Down"; } }

	private Rigidbody2D rb;
	private Animator animator;
	private Animator child_ChestAnimator;
	private Animator child_LegAnimator;
	private Animator child_ShoeAnimator;
	private Animator child_HatAnimator;

	// Grab the equipped items dictionary from PlayerData
	// The PlayerData is a singleton
	// PlayerData.Instance.equipped_items, PlayerData.Instance.original_load_items
	private PlayerData playerData => PlayerData.Instance;
	private List<int> equipped_items => playerData.equipped_items;

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
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if(GetComponent<RobotDepth>()==null)gameObject.AddComponent<RobotDepth>();
		animator = GetComponent<Animator>();
		child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();
		child_LegAnimator = transform.GetChild(1).GetComponent<Animator>();
		child_ShoeAnimator = transform.GetChild(2).GetComponent<Animator>();
		child_HatAnimator = transform.GetChild(3).GetComponent<Animator>();

		CreateAnimationDictionary();

		lastMovementInputDirection = Vector2.down;

		if (playerData != null)
		{
			ApplyEquippedCosmetics();
		}
	}

	private void OnEnable()
	{
		if (PlayerData.Instance != null)
		{
			PlayerData.Instance.EquippedItemsChanged += ApplyEquippedCosmetics;
		}
	}

	private void OnDisable()
	{
		if (PlayerData.Instance != null)
		{
			PlayerData.Instance.EquippedItemsChanged -= ApplyEquippedCosmetics;
		}
	}

	// Apply the equipped cosmetics to the child animators. X99 ids
	// (199/299/399/499) mark an empty slot, so its controller is cleared
	private void ApplyEquippedCosmetics()
	{
		// Organize equippedItems by lowest to highest id
		equipped_items.Sort();

		for (int i = 0; i < equipped_items.Count; i++)
		{
			if (equipped_items[i] >= 100 && equipped_items[i] < 200)
			{
				if (equipped_items[i] == 199)
				{
					child_HatAnimator.runtimeAnimatorController = null;
				}
				else
				{
					SetHatSprite(cosmeticHandler.GetHatController(equipped_items[i] - 100));
				}
			}
			if (equipped_items[i] >= 200 && equipped_items[i] < 300)
			{
				if (equipped_items[i] == 299)
				{
					child_ChestAnimator.runtimeAnimatorController = null;
				}
				else
				{
					SetChestSprite(cosmeticHandler.GetChestController(equipped_items[i] - 200));
				}
			}
			if (equipped_items[i] >= 300 && equipped_items[i] < 400)
			{
				if (equipped_items[i] == 399)
				{
					child_LegAnimator.runtimeAnimatorController = null;
				}
				else
				{
					SetLegSprite(cosmeticHandler.GetLegController(equipped_items[i] - 300));
				}
			}
			if (equipped_items[i] >= 400 && equipped_items[i] < 500)
			{
				if (equipped_items[i] == 499)
				{
					child_ShoeAnimator.runtimeAnimatorController = null;
				}
				else
				{
					SetShoeSprite(cosmeticHandler.GetShoeController(equipped_items[i] - 400));
				}
			}
		}
	}

	// Use the Start() method
	private void Start()
	{
        gameObject.AddComponent<LocalRobotPresentation>();
        gameObject.AddComponent<DesktopHudLayout>();
		if (GameManager.Instance != null)
		{
			transform.position = GameManager.Instance.playerSpawnPosition;
		}
	}

	// Can't move function
	public void StopPlayer()
	{
		// Stop the player from moving
		canMove = false;
        sprint.Reset();
        touchInput = Vector2.zero;
        if (rb != null) rb.velocity = Vector2.zero;

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

	// Moving Up (Joystick)
	public void MoveUp()
	{
		// Essentially, Move Up and Stop
		// Set the movement input direction to up
		if (!CanMove) return;
        touchInput = Vector2.up;
        movementInputDirection = touchInput;

		// Update the animator
		UpdateAnimator();
	}

	// Moving Down (Joystick)
	public void MoveDown()
	{
		if (!CanMove) return;
        touchInput = Vector2.down;
        movementInputDirection = touchInput;

		// Update the animator
		UpdateAnimator();
	}

	// Moving Left (Joystick)
	public void MoveLeft()
	{
		if (!CanMove) return;
        touchInput = Vector2.left;
        movementInputDirection = touchInput;

		// Update the animator
		UpdateAnimator();
	}

	// Moving Right (Joystick)
	public void MoveRight()
	{
		if (!CanMove) return;
        touchInput = Vector2.right;
        movementInputDirection = touchInput;

		// Update the animator
		UpdateAnimator();
	}

	// Stopping the player
	public void StopMoving()
	{
		sprint.Reset();
        touchInput = Vector2.zero;
        movementInputDirection = Vector2.zero;
        if (rb != null) rb.velocity = Vector2.zero;
	}

	// Use update for animations
	private void Update()
	{
		// Update the player's animations
		// (cosmetic changes are handled by the EquippedItemsChanged event)
		if (CanMove)
		{
            Vector2 keyboard = GetInput();
            movementInputDirection = keyboard != Vector2.zero ? keyboard : touchInput;
            sprint.Tick(movementInputDirection != Vector2.zero, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift), Time.deltaTime);
			UpdateAnimator();
		}
        else { StopMoving(); UpdateAnimator(); }
	}

	// Dance emote function
	// Check the player's equipped items and set the dance emote
	// If 500, headripper (3), 501, robotdance (2), 502, zenflip (1)
	public void DanceEmote()
	{
		// Check the equipped items
		// If the player has the headripper, robotdance, or zenflip
		// Set the dance emote accordingly
		if (equipped_items.Contains(500))
		{
			PerformEmote(3);
		}
		else if (equipped_items.Contains(501))
		{
			PerformEmote(2);
		}
		else if (equipped_items.Contains(502))
		{
			PerformEmote(1);
		}
	}

	private void OnApplicationFocus(bool focused) { if (!focused) StopMoving(); }
    private void OnApplicationPause(bool paused) { if (paused) StopMoving(); }
	private void FixedUpdate()
	{
		if(CanMove)
            HandleMovement(movementInputDirection);
        else{
            Vector2 movementInput = Vector2.zero;
            HandleMovement(movementInput);
            isEmoting = false;
        }
	}

    // New method to toggle player movement
    public void ToggleMovement()
    {
        if(canMove) StopPlayer(); else UnstopPlayer();
    }

	private void ChangeAnimationState(Animator animator, string newState, ref string currentState)
	{
		if (animator == null || animator.runtimeAnimatorController == null || currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}

	private void ChangePlayerAnimationState(string newState)
	{
		ChangeAnimationState(animator, newState, ref currentState);
	}

	private void ChangeChestAnimationState(string newState)
	{
		ChangeAnimationState(child_ChestAnimator, newState, ref chestState);
	}

	private void ChangeLegAnimationState(string newState)
	{
		ChangeAnimationState(child_LegAnimator, newState, ref legState);
	}

	private void ChangeShoeAnimationState(string newState)
	{
		ChangeAnimationState(child_ShoeAnimator, newState, ref shoeState);
	}
	private void ChangeHatAnimationState(string newState)
	{
		ChangeAnimationState(child_HatAnimator, newState, ref hatState);
	}

	private Vector2 GetInput()
	{
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");
		if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (selected != null && (selected.GetComponent<TMPro.TMP_InputField>() != null || selected.GetComponent<UnityEngine.UI.InputField>() != null)) return Vector2.zero;
        }
        return Mathf.Abs(moveHorizontal) > 0 ? new Vector2(Mathf.Sign(moveHorizontal), 0) : new Vector2(0, Mathf.Sign(moveVertical) * (moveVertical == 0 ? 0 : 1));
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

			// Stops CoRoutines iff emoting
			if (isEmoting)
			{
				StopEmoting();
			}
			isEmoting = false;
		}

		rb.velocity = movement * charSpeed * (IsSprinting ? SprintState.Multiplier : 1f);
        float pace = IsSprinting && movement != Vector2.zero ? 1.4f : 1f;
        animator.speed = child_ChestAnimator.speed = child_LegAnimator.speed = child_ShoeAnimator.speed = child_HatAnimator.speed = pace;
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
		if (emoteRoutine != null) StopCoroutine(emoteRoutine);
        emoteRoutine = StartCoroutine(WaitForEmoteToFinish(emoteIndex));
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
        if (emoteRoutine != null) StopCoroutine(emoteRoutine);
        emoteRoutine = null;
		isEmoting = false;

		ChangePlayerAnimationState("Char_Idle_Down");
		ChangeChestAnimationState("Chest_Idle_Down");
		ChangeLegAnimationState(legIdleAnimations[lastMovementInputDirection]);
		ChangeShoeAnimationState(shoeIdleAnimations[lastMovementInputDirection]);
		ChangeHatAnimationState(hatIdleAnimations[lastMovementInputDirection]);
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
}
