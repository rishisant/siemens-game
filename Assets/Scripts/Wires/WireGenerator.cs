using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    // Prefabs to Instantiate what we need
    public GameObject wireEntry;
    public GameObject wirePlug;

    public int level;

    public Vector3[] line2ndPointSpawns = {
        new Vector3(-10, 5, 0),
        new Vector3(-10, 3, 0),
        new Vector3(-10, 1, 0),
        new Vector3(-10, -1, 0),
        new Vector3(-10, -3, 0),
        new Vector3(-10, -5, 0)
    };
    public Vector3[] entrySpawns = {
        new Vector3(-12, 5, 0),
        new Vector3(-12, 3, 0),
        new Vector3(-12, 1, 0),
        new Vector3(-12, -1, 0),
        new Vector3(-12, -3, 0),
        new Vector3(-12, -5, 0)
    };
    public Vector3[] exitSpawns = {
        new Vector3(12, 5, 0),
        new Vector3(12, 3, 0),
        new Vector3(12, 1, 0),
        new Vector3(12, -1, 0),
        new Vector3(12, -3, 0),
        new Vector3(12, -5, 0)
    };
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

    // keep track of times for different levels
    private List<System.TimeSpan> allTimes = new List<System.TimeSpan>();

    public GameOverManager gameOverManager;

    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkConnection())
        {
            stopwatch.Stop();
            allTimes.Add(stopwatch.Elapsed);
            if (level < 6)
            {
                // FIXME: add another background for going to next level
            }
            else
            {
                gameOverManager.Setup(stopwatch.Elapsed);
            }
        }
    }

    void StartLevel()
    {
        // FIXME: move logic out of Start and into StartLevel so that we can
        // have different levels
        level++;
        spawnObjects();
        stopwatch.Start();
    }

    void clearWires()
    {
        foreach (GameObject g in allWires)
        {
            Destroy(g);
        }
        allWires.Clear();
    }

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

    private T[] shuffle<T>(T[] sourceArray, int numElements)
    {
        // https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
        // Fisher-Yates algorithm
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