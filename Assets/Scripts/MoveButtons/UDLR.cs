using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @class UDLR
 * @brief UDLR (up down left right) handles the buttons to move the player
 * note that OnClick of the button is already on the Movement of the Player
 *
 * @see Character_Movement
 */
public class UDLR : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject player;
    private Character_Movement playerMovement;
    private bool isMovingUp, isMovingDown, isMovingLeft, isMovingRight;

    private void Awake()
    {
        playerMovement = player.GetComponent<Character_Movement>();
    }

    private void Update()
    {
        if (isMovingUp)
            playerMovement.MoveUp();
        if (isMovingDown)
            playerMovement.MoveDown();
        if (isMovingLeft)
            playerMovement.MoveLeft();
        if (isMovingRight)
            playerMovement.MoveRight();
        if (!isMovingUp && !isMovingDown && !isMovingLeft && !isMovingRight)
            playerMovement.StopMoving();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Start moving based on the button pressed
        switch (eventData.pointerPress.name)
        {
            case "UpButton":
                isMovingUp = true;
                break;
            case "DownButton":
                isMovingDown = true;
                break;
            case "LeftButton":
                isMovingLeft = true;
                break;
            case "RightButton":
                isMovingRight = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Stop movement when the button is released
        isMovingUp = false;
        isMovingDown = false;
        isMovingLeft = false;
        isMovingRight = false;
    }
}
