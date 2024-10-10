// Rishi Santhanam
// CSCE 482 Siemens Gamification

using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
	[SerializeField] private float charSpeed = 4f;

	private string currentState;

	private Rigidbody2D rb;
	private Animator animator;
	private Animator child_ChestAnimator;

	private Vector2 movementInputDirection;
	private Vector2 lastMovementInputDirection;


	private Dictionary<Vector2, string> playerDirectionAnimations;
	private Dictionary<Vector2, string> playerIdleAnimations;

	private Dictionary<Vector2, string> chestDirectionAnimations;
	private Dictionary<Vector2, string> chestIdleAnimations;


	private bool syncFlag = false;



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
	}

	// Use update for animations
	private void Update()
	{
		// Update the animator
		movementInputDirection = GetInput();
		UpdateAnimator();
		//Debug.Log("Parent: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		//Debug.Log("Child: " + child_ChestAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

	}

	private void FixedUpdate()
	{
		HandleMovement();
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

	private void HandleMovement()
	{
		rb.velocity = movementInputDirection * charSpeed;
	}

	private void UpdateAnimator()
	{
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
}
