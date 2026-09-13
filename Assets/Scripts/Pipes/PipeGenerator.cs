using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/**
 * @class PipeGenerator
 * @brief Main driver class for generating and checking solutions for the pipe
 * game
 * @details This handles generation, moving onto the next level, checking
 * solutions, as well as maintaining global game state in memory as well as
 * what is happening on the screen
 *
 * @see PipeBehavior
 * @see PipeInfo
 */
public class PipeGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public System.Diagnostics.Stopwatch gameTime = new System.Diagnostics.Stopwatch();

    public GameObject source;
    public GameObject sink;
    public GameObject straightPipe;
    public GameObject turnPipe;

    int level = 1;
    private PuzzleHUD hud;
    private bool checking;
    private bool completed;
    private ArcadeFeedback arcade;
    private int turns, attempts;
    private GameObject flowParticle;
    private readonly Dictionary<Vector2Int, GameObject> cells = new Dictionary<Vector2Int, GameObject>();
    private readonly Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

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

    private static PipeInfo[] emptyRow = new PipeInfo[] {new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty), new PipeInfo(Direction.up, PipeType.empty)};

    private Vector3[][] spawnLocations = new Vector3[][] {
        new Vector3[] {new Vector3(-7, 4, 0), new Vector3(-5, 4, 0), new Vector3(-3, 4, 0), new Vector3(-1, 4, 0), new Vector3(1, 4, 0), new Vector3(3, 4, 0), new Vector3(5, 4, 0), new Vector3(7, 4, 0)},
        new Vector3[] {new Vector3(-7, 2, 0), new Vector3(-5, 2, 0), new Vector3(-3, 2, 0), new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(3, 2, 0), new Vector3(5, 2, 0), new Vector3(7, 2, 0)},
        new Vector3[] {new Vector3(-7, 0, 0), new Vector3(-5, 0, 0), new Vector3(-3, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(3, 0, 0), new Vector3(5, 0, 0), new Vector3(7, 0, 0)},
        new Vector3[] {new Vector3(-7, -2, 0), new Vector3(-5, -2, 0), new Vector3(-3, -2, 0), new Vector3(-1, -2, 0), new Vector3(1, -2, 0), new Vector3(3, -2, 0), new Vector3(5, -2, 0), new Vector3(7, -2, 0)},
        new Vector3[] {new Vector3(-7, -4, 0), new Vector3(-5, -4, 0), new Vector3(-3, -4, 0), new Vector3(-1, -4, 0), new Vector3(1, -4, 0), new Vector3(3, -4, 0), new Vector3(5, -4, 0), new Vector3(7, -4, 0)}
    };

    private PipeInfo[][][] easyLevels = {
        PipeBoards.Create(new Vector2Int(1,0), new Vector2Int(1,7)),
        PipeBoards.Create(new Vector2Int(1,0), new Vector2Int(1,5), new Vector2Int(3,5))
    };
    private PipeInfo[][][] mediumLevels = {
        PipeBoards.Create(new Vector2Int(0,0), new Vector2Int(0,6), new Vector2Int(3,6), new Vector2Int(3,1)),
        PipeBoards.Create(new Vector2Int(1,1), new Vector2Int(1,7), new Vector2Int(4,7), new Vector2Int(4,0))
    };
    private PipeInfo[][][] hardLevels = {
        PipeBoards.Create(new Vector2Int(0,0), new Vector2Int(0,7), new Vector2Int(4,7), new Vector2Int(4,1), new Vector2Int(2,1), new Vector2Int(2,5))
    };

    private PipeInfo[][] currentLevel;

    private ArrayList gameObjects = new ArrayList();

    public PipeGameOverManager pipeGameOverManager;

    public Button checkSolutionButton;

    // Start is called before the first frame update
    void Start()
    {
        arcade = gameObject.AddComponent<ArcadeFeedback>();
        hud = gameObject.AddComponent<PuzzleHUD>();
        hud.Initialize("PECULIAR PIPES", "Tap a pipe to rotate it. Follow the arrows from inlet to outlet.", 9f, 5.5f);
        hud.SetPrimaryButton(checkSolutionButton, "Test flow");
        gameTime.Start();
        GenerateLevel(easyLevels[rand.Next(2)]);
    }

    /**
     * Since turnPipes have a 0.5 offset for alignment reasons, as well as
     * source and sink might cause issues later, so we will have this function
     * to get spawn position based on type
     *
     * for turn pipes, we also have to do some modifications to the
     * transform.position so that we are always aligned to the grid
     *
     * @see PipeBehavior.RotatePipe
     */
    Vector3 getSpawnLocation(PipeType type, int i, int j, int dir)
    {
        if (type == PipeType.turn) {
            Vector3 res = spawnLocations[i][j];
            res.x += 0.5f;
            switch (dir) {
                case 0:
                    res.y -= 1f;
                    break;
                case 1:
                    res.x -= 1f;
                    res.y -= 1f;
                    break;
                case 2:
                    res.x -= 1f;
                    break;
            }
            return res;
        }
        else {
            return spawnLocations[i][j];
        }
    }

    Quaternion getSpawnRotation(PipeInfo[][] currLevel, int i, int j)
    {
        Direction rotation = currLevel[i][j].direction;
        switch (rotation)
        {
            case Direction.up:
                return possibleRotations[0];
            case Direction.right:
                return possibleRotations[1];
            case Direction.down:
                return possibleRotations[2];
            case Direction.left:
                return possibleRotations[3];
        }
        return possibleRotations[0];
    }

    void InstantiatePipe(PipeInfo[][] currLevel, GameObject prefab, Vector3 spawn, Quaternion rotation, int row, int col, int dir, PipeType type)
    {
        GameObject pipe = Instantiate(prefab, spawn, rotation);
        PipeBehavior pipeBehavior = pipe.GetComponent<PipeBehavior>();
        pipeBehavior.generator = this;
        cells[new Vector2Int(row,col)] = pipe;
        pipeBehavior.gameState = currLevel;
        pipeBehavior.row = row;
        pipeBehavior.col = col;
        pipeBehavior.pipeInfo = new PipeInfo((Direction)dir, type);
        currLevel[row][col].direction = (Direction)dir;
        gameObjects.Add(pipe);

        // the sprites for right / left are flipped on straight pipes, make sure to handle this properly
        if ((dir == 1 || dir == 3) && type != PipeType.turn)
        {
            SpriteRenderer sr = pipe.GetComponent<SpriteRenderer>();
            sr.flipY = !sr.flipY;
        }
    }

    void GenerateLevel(PipeInfo[][] currLevel)
    {
        var copy = new PipeInfo[currLevel.Length][];
        for(int r=0;r<currLevel.Length;r++)
        {
            copy[r] = new PipeInfo[currLevel[r].Length];
            for(int c=0;c<currLevel[r].Length;c++) copy[r][c] = new PipeInfo(currLevel[r][c].direction,currLevel[r][c].type);
        }
        currLevel = copy;
        currentLevel = currLevel;
        turns=0;attempts=0;hud.SetFlow(0);
        hud.SetProgress("BOARD " + level + " / 3  ·  " + (level == 1 ? "POWER THE WORKSHOP" : level == 2 ? "WAKE THE COOLANT PUMPS" : "BRING THE LAB ONLINE"));
        for (int i = 0; i < currLevel.Length; i++)
        {
            for (int j = 0; j < currLevel[i].Length; j++)
            {
                int dir = rand.Next(4);
                Quaternion currRotation = possibleRotations[dir];

                switch (currLevel[i][j].type)
                {
                    case PipeType.straight:
                        InstantiatePipe(currLevel, straightPipe, getSpawnLocation(PipeType.straight, i, j, dir), currRotation, i, j, dir, PipeType.straight);
                        break;
                    case PipeType.turn:
                        // with turnpipes, right is left and left is right in terms of animations
                        // this is due to the rotate 90 working differently for the shape i've defined
                        // this will handle whether it is left or right, flip it accordingly, and sync the direction in the gamestate
                        if (dir == 1)
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), possibleRotations[3], i, j, dir, PipeType.turn);
                        }
                        else if (dir == 3)
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), possibleRotations[1], i, j, dir, PipeType.turn);

                        }
                        else
                        {
                            InstantiatePipe(currLevel, turnPipe, getSpawnLocation(PipeType.turn, i, j, dir), currRotation, i, j, dir, PipeType.turn);
                        }
                        break;
                    case PipeType.source:
                        Quaternion rot = getSpawnRotation(currLevel, i, j);
                        GameObject bruh1 = Instantiate(source, getSpawnLocation(PipeType.source, i, j, 0), rot);

                        // the source has the same sprites as straight pipe, so we also need to flip if its right or left
                        if (rot == possibleRotations[1] || rot == possibleRotations[3])
                        {
                            SpriteRenderer sr = bruh1.GetComponent<SpriteRenderer>();
                            sr.flipY = !sr.flipY;
                        }

                        cells[new Vector2Int(i,j)]=bruh1;
                        gameObjects.Add(bruh1);
                        break;
                    case PipeType.sink:
                        GameObject bruh2 = Instantiate(sink, getSpawnLocation(PipeType.sink, i, j, 0), getSpawnRotation(currLevel, i, j));
                        cells[new Vector2Int(i,j)]=bruh2;
                        gameObjects.Add(bruh2);
                        break;
                }
            }
        }
    }

    void ClearLevel()
    {
        foreach (GameObject g in gameObjects)
        {
            Destroy(g);
        }
        if(flowParticle!=null)flowParticle.SetActive(false);
        gameObjects.Clear();
        cells.Clear();
        originalColors.Clear();
    }

    public bool CheckSolution(PipeInfo[][] currLevel)
    {
        List<Vector2Int> path;
        Vector2Int failure;
        return PipePath.Trace(currLevel, out path, out failure);
    }

    private void Update() { if (hud != null) hud.SetTime(gameTime.Elapsed); }
    private void OnApplicationPause(bool paused)
    {
        if (paused) gameTime.Stop(); else if (!completed) gameTime.Start();
    }
    public void OnPipeRotated()
    {
        foreach (var pair in originalColors) if (pair.Key != null) pair.Key.color = pair.Value;
        originalColors.Clear();
        turns++;
        arcade.Tone(330+(turns%4)*55,.035f);
        hud.SetFeedback("Valve clicks: "+turns+"  /  Route the coolant, then test the flow.");
    }
    private void Tint(Vector2Int cell, Color color)
    {
        GameObject pipe;
        if (!cells.TryGetValue(cell,out pipe)) return;
        foreach (var renderer in pipe.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!originalColors.ContainsKey(renderer)) originalColors[renderer] = renderer.color;
            renderer.color = color;
        }
    }
    private IEnumerator TestFlow()
    {
        checking = true;
        checkSolutionButton.interactable = false;
        foreach(var pair in originalColors)if(pair.Key!=null)pair.Key.color=pair.Value;
        originalColors.Clear();attempts++;hud.SetFlow(0);
        hud.SetFeedback("PUMP STARTED. Charging the circuit...");
        arcade.Tone(150,.18f);
        List<Vector2Int> path;
        Vector2Int failure;
        bool solved = PipePath.Trace(currentLevel, out path, out failure);
        foreach (var pipe in cells.Values) {var behavior=pipe.GetComponent<PipeBehavior>();if(behavior!=null)behavior.enabled=false;}
        foreach (var cell in path)
        {
            Tint(cell, new Color(.4f,1f,.84f));
            hud.SetFlow((path.IndexOf(cell)+1f)/path.Count);
            arcade.Tone(300+path.IndexOf(cell)*24,.055f);
            hud.SetFeedback("COOLANT FLOW  "+(path.IndexOf(cell)+1)+" / "+path.Count+"  /  Keep it moving!");
            yield return FlowThrough(cell,.12f);
        }
        if (solved)
        {
            hud.SetFeedback(attempts==1?"FIRST-TRY FIX! Circuit online.":"CIRCUIT ONLINE! Byte City is humming.");
            arcade.Tone(880,.25f);hud.Celebrate();
            gameTime.Stop();
            yield return new WaitForSeconds(.85f);
            ClearLevel(); level++;
            if (level == 2) GenerateLevel(mediumLevels[rand.Next(mediumLevels.Length)]);
            else if (level == 3) GenerateLevel(hardLevels[0]);
            else { completed = true; pipeGameOverManager.Setup(gameTime.Elapsed); }
            if (!completed) { gameTime.Start(); hud.SetFeedback("New board. Rotate the pipes to reconnect the flow."); }
        }
        else
        {
            Tint(failure, new Color(1,0.35f,0.3f));
            arcade.Tone(100,.2f);
            hud.SetFeedback("Flow stopped here. Check the next pipe's inlet and arrow.", true);
            foreach (var pipe in cells.Values) {var behavior=pipe.GetComponent<PipeBehavior>();if(behavior!=null)behavior.enabled=true;}
        }
        checking = false;
        checkSolutionButton.interactable = !completed;
    }

    private IEnumerator FlowThrough(Vector2Int cell,float duration)
    {
        if(flowParticle==null)
        {
            flowParticle=new GameObject("Coolant pulse");flowParticle.transform.SetParent(transform);flowParticle.SetActive(false);
            var sr=flowParticle.AddComponent<SpriteRenderer>();
            sr.sprite=ByteCityTheme.Current.button;sr.color=new Color(.4f,1,.9f);sr.sortingOrder=100;
            float width=sr.sprite.bounds.size.x;flowParticle.transform.localScale=Vector3.one*(.28f/width);
        }
        Vector3 end=spawnLocations[cell.x][cell.y]+Vector3.down*.5f;
        Vector3 start=flowParticle.activeSelf?flowParticle.transform.position:end;
        if(start==Vector3.zero)start=end;
        flowParticle.SetActive(true);
        for(float t=0;t<duration;t+=Time.deltaTime){flowParticle.transform.position=Vector3.Lerp(start,end,t/duration);yield return null;}
        flowParticle.transform.position=end;
    }

    public IEnumerator ChangeButtonColorOnFail()
    {
        Image buttonImage = checkSolutionButton.GetComponent<Image>();

        buttonImage.color = Color.red;

        yield return new WaitForSeconds(1);

        buttonImage.color = Color.white;

    }

    public void CheckSolutionButton()
    {
        if (!checking && !completed) StartCoroutine(TestFlow());
    }
}
