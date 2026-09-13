using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class PipeBehavior
 * @brief This handles the behavior of an individual pipe
 * @details When clicking on a pipe, the pipe rotates 90 degrees. There is also
 * synchronization with the global game state that lives in PipeGenerator. When
 * clicking turn pipes, the sprite image needs to be flipped to keep it
 * consistent with what's displayed at the screen
 *
 * @see PipeGenerator
 */
public class PipeBehavior : MonoBehaviour
{
    public PipeInfo pipeInfo;
    public PipeGenerator generator;

    private bool mouseDown;
    private bool movable;

    // variables to keep track of main game state
    public PipeInfo[][] gameState;
    public int row;
    public int col;

    private Vector3 rotate90 = new Vector3(0, 0, 90);
    private Vector3 rotateTurnPipe = new Vector3(0, 0, 270);

    private Quaternion[] possibleRotations = {
        // up
        Quaternion.Euler(0, 0, 0),
        // right
        Quaternion.Euler(0, 0, 90),
        // down
        Quaternion.Euler(0, 0, 180),
        // left
        Quaternion.Euler(0, 0, -90)
    };

    // Start is called before the first frame update
    void Start()
    {
        // pipeInfo is populated in pipeGenerator
        mouseDown = false;
        movable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        movable = true;
    }

    void OnMouseExit()
    {
        movable = false;
    }

    void OnMouseDown()
    {
        if (!enabled || (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())) return;
        mouseDown = true;
        movable = true;
    }

    void OnMouseUp()
    {
        bool pressed = mouseDown;
        mouseDown = false;
        if (!enabled || !pressed || !movable)
        {
            return;
        }
        mouseDown = false;
        RotatePipe();
        if (generator != null) generator.OnPipeRotated();
    }

    void RotatePipe()
    {
        // goes to next clockwise direction in a cycle
        int dir = (((int)pipeInfo.direction) + 1) % 4;
        Direction newDir = ((Direction)dir);

        if (pipeInfo.type == PipeType.turn)
        {
            // when turning turnpipes, the direction they rotate has to go in the other way with how we defined Direction
            // The position changes handle the animations so that they stay in the same spot even after rotating
            switch (newDir)
            {
                case Direction.up:
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1f, gameObject.transform.position.z);
                    gameObject.transform.Rotate(rotateTurnPipe);
                    break;
                case Direction.right:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
                case Direction.down:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z);
                    break;
                case Direction.left:
                    gameObject.transform.Rotate(rotateTurnPipe);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1f, gameObject.transform.position.y, gameObject.transform.position.z);
                    break;
            }
        }
        else
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.flipY = !sr.flipY;
            gameObject.transform.Rotate(rotate90);
        }

        pipeInfo.direction = newDir;
        gameState[row][col].direction = newDir;
    }
}
