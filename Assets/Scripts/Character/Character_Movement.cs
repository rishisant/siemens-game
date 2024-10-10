// Rishi Santhanam
// CSCE 482 Siemens Gamification

using System;
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
	private Animator child_FootAnimator;
	private Animator child_HeadAnimator;

	private Vector2 movementInputDirection;
	private Vector2 lastMovementInputDirection;


	private Dictionary<Vector2, string> playerDirectionAnimations;
	private Dictionary<Vector2, string> playerIdleAnimations;

	private Dictionary<Vector2, string> chestDirectionAnimations;
	private Dictionary<Vector2, string> chestIdleAnimations;


	private bool syncFlag = false;
	private bool isResettingAnimations;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();

		playerDirectionAnimations = new Dictionary<Vector2, string>
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

		chestDirectionAnimations = new Dictionary<Vector2, string>
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

		lastMovementInputDirection = Vector2.down;

		SetChestSprite(cosmeticHandler.GetChestController(0));
	}

	// Use update for animations
	private void Update()
	{
		// Update the animator
		movementInputDirection = GetInput();
		UpdateAnimator();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (child_ChestAnimator.runtimeAnimatorController == cosmeticHandler.GetChestController(0))
			{
				SetChestSprite(cosmeticHandler.GetChestController(1));
			}
			else
			{
				SetChestSprite(cosmeticHandler.GetChestController(0));
			}
		}
	}

	private void FixedUpdate()
	{
		HandleMovement(movementInputDirection);
	}

	private void ChangePlayerAnimationState(string newState)
	{
		if (currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}
	private void ChangeChestAnimationState(string newState)
	{
		if (currentState == newState) return;

		child_ChestAnimator.Play(newState);
		currentState = newState;
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

		rb.velocity = movement * charSpeed;
	}

	private void UpdateAnimator()
	{
		if (isResettingAnimations) return;

		if (movementInputDirection != Vector2.zero)
		{
			if (playerDirectionAnimations.ContainsKey(movementInputDirection))
			{
				ChangePlayerAnimationState(playerDirectionAnimations[movementInputDirection]);
				ChangeChestAnimationState(chestDirectionAnimations[movementInputDirection]);
				lastMovementInputDirection = movementInputDirection;
			}

		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				ChangePlayerAnimationState(playerIdleAnimations[lastMovementInputDirection]);
				ChangeChestAnimationState(chestIdleAnimations[lastMovementInputDirection]);
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
		else
		{
			Debug.LogWarning("The provided chest prefab does not have an Animator component.");
		}
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
			if (playerDirectionAnimations.ContainsKey(movement))
			{
				animator.Play(playerDirectionAnimations[movement], 0, 0f);
				child_ChestAnimator.Play(chestDirectionAnimations[movement], 0, 0f);
				lastMovementInputDirection = movement;

				Debug.Log("Playing currentInput: " + chestDirectionAnimations[movement]);
			}

		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				animator.Play(playerIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_ChestAnimator.Play(chestIdleAnimations[lastMovementInputDirection], 0, 0f);
			}

			Debug.Log("Playing lastInput: " + chestIdleAnimations[lastMovementInputDirection]);
		}


		isResettingAnimations = false;
	}
}
