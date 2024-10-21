using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Driver class for WireGame generation and success criteria.
 */
public class WireGenerator : MonoBehaviour
{
    // For spawn locations
    public System.Random rand = new System.Random();

    // For section completion timing
    public System.Diagnostics.Stopwatch levelStopwatch = new System.Diagnostics.Stopwatch();

    // For game completion timing
    public System.Diagnostics.Stopwatch gameStopwatch = new System.Diagnostics.Stopwatch();

    // Prefabs to Instantiate what we need
    public GameObject wireEntry;
    public GameObject wirePlug;

    public int level;

    // Collection of the line renderers 2nd point, is at the same as the wireLength in Unity
    public Vector3[] line2ndPointSpawns = {
        new Vector3(-10, 5, 0),
        new Vector3(-10, 3, 0),
        new Vector3(-10, 1, 0),
        new Vector3(-10, -1, 0),
        new Vector3(-10, -3, 0),
        new Vector3(-10, -5, 0)
    };

    // Collection of the possible wireEntry spawn locations
    public Vector3[] entrySpawns = {
        new Vector3(-12, 5, 0),
        new Vector3(-12, 3, 0),
        new Vector3(-12, 1, 0),
        new Vector3(-12, -1, 0),
        new Vector3(-12, -3, 0),
        new Vector3(-12, -5, 0)
    };

    // Collection of the possible wirePlug spawn locations
    public Vector3[] exitSpawns = {
        new Vector3(12, 5, 0),
        new Vector3(12, 3, 0),
        new Vector3(12, 1, 0),
        new Vector3(12, -1, 0),
        new Vector3(12, -3, 0),
        new Vector3(12, -5, 0)
    };

    // Collection of possible wire colors
    public Color[] colors = {
        Color.red,
        Color.green,
        Color.yellow,
        Color.blue,
        Color.white,
        Color.magenta
    };

    // keep track of all plug stats to check when all are connected
    private List<PlugStats> allPlugStats = new List<PlugStats>();

    // keep track of all objects for cleanup in between levels
    private List<GameObject> allWires = new List<GameObject>();

    // Level over screen manager member
    public LevelOverManager levelOverManager;

    // Game over screen manager member
    public GameOverManager gameOverManager;

    /**
     * Start() is a Unity function that runs before the first frame update of this scene
     * This method just starts the first level by calling the StartLevel() function
     */
    void Start()
    {
        level = 0;
        gameStopwatch.Start();
        StartLevel();
    }

    /**
     * Update() is a Unity function that runs on every frame update.
     * This function checks the connections of the wires, and if they are all connected
     * then the gameOverManager member is called to end the level
     * If performance becomes an issue, checkConnection() can be moved to the wire
     * connection behavior function.
     */
    void Update()
    {
        if (checkConnection())
        {
            // FIXME: change back to 6 after testing
            if (level < 2)
            {
                levelStopwatch.Stop();
                levelOverManager.Setup(levelStopwatch.Elapsed);
            }
            else
            {
                // FIXME: setup backend call
                levelStopwatch.Stop();
                gameStopwatch.Stop();
                gameOverManager.Setup(levelStopwatch.Elapsed, gameStopwatch.Elapsed);
            }
        }
    }

    /**
     * StartLevel() contains the logic for starting the level and is called at Start()
     * and whenever a new level needs to be generated
     */
    public void StartLevel()
    {
        clearWires();
        level++;
        spawnObjects();
        levelStopwatch.Restart();
    }

    /**
     * clearWires() destroys all game objects to prepare for the next level
     * makes use of the allWires class member to keep track and destroy gameObjects
     */
    void clearWires()
    {
        foreach (GameObject g in allWires)
        {
            Destroy(g);
        }
        allWires.Clear();
    }

    /**
     * spawnObjects() spawns the objects for the game to function.
     * This starts by shuffling the colors and exit spawns, then instantiates `level`
     * wires.
     */
    void spawnObjects()
    {
        Color[] shuffledColors = shuffle(colors, colors.Length);
        Vector3[] shuffledExitSpawns = shuffle(exitSpawns, exitSpawns.Length);
        for (int i = 0; i < level; i++)
        {
            Color currColor = shuffledColors[i];
            GameObject entry = Instantiate(wireEntry, entrySpawns[i], wireEntry.transform.rotation);

            foreach (Transform child in entry.transform)
            {
                SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                childSpriteRenderer.material.color = currColor;
                childSpriteRenderer.color = currColor;
            }

            LineRenderer line = entry.GetComponent<LineRenderer>();
            line.SetPosition(0, entrySpawns[i]);
            line.SetPosition(1, line2ndPointSpawns[i]);
            line.material.color = currColor;
            line.startColor = currColor;
            line.endColor = currColor;

            GameObject plug = Instantiate(wirePlug, shuffledExitSpawns[i], wirePlug.transform.rotation);
            SpriteRenderer plugSpriteRenderer = plug.GetComponent<SpriteRenderer>();
            plugSpriteRenderer.color = currColor;

            // add now for game complete check
            PlugStats plugStats = plug.GetComponent<PlugStats>();
            allPlugStats.Add(plugStats);

            // keep track of objects for clearWires() function
            allWires.Add(entry);
            allWires.Add(plug);
        }
    }

    /**
     * shuffle() is a template function that shuffles an array
     * This is to be used in the spawnObjects() function so that
     * we can get random positions for the colors and exit spawns
     * The Fisher-Yates algorithm is utilized in this function
     * 
     * @param sourceArray The source array that wants to be shuffled
     * @param numElements The number of elements in the source array
     * @return            A new array that is a shuffled version of the source Array
     * @see               https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
     */
    private T[] shuffle<T>(T[] sourceArray, int numElements)
    {
        T[] copy = new T[numElements];
        sourceArray.CopyTo(copy, 0);

        int n = sourceArray.Length;
        while (n > 1)
        {
            int k = rand.Next(n--);
            T temp = copy[n];
            copy[n] = copy[k];
            copy[k] = temp;
        }

        return copy;
    }

    /**
     * checkConnection() checks the connection of all the wires in the allPlugStats member
     * 
     * @return a boolean, true if all wires are connected, false otherwise
     */
    private bool checkConnection()
    {
        foreach (PlugStats p in allPlugStats)
        {
            if (!p.connected)
            {
                return false;
            }
        }
        return true;
    }

}
