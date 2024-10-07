using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public GameObject wireEntry;
    public GameObject wirePlug;

    public int level = 1;

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
        Color.black
    };

    private List<PlugStats> allPlugStats = new List<PlugStats>();

    // Start is called before the first frame update
    void Start()
    {
        spawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkConnection())
        {
            Debug.Log("game is finished");
        }
    }

    void spawnObjects()
    {
        Color[] shuffledColors = shuffle(colors, colors.Length);
        Vector3[] shuffledExitSpawns = shuffle(exitSpawns, exitSpawns.Length);
        for (int i = 0; i < 6; i++) {
            Color currColor = shuffledColors[i];
            GameObject entry = Instantiate(wireEntry, entrySpawns[i], wireEntry.transform.rotation);

            foreach (Transform child in entry.transform)
            {
                SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                childSpriteRenderer.color = currColor;
            }

            LineRenderer line = entry.GetComponent<LineRenderer>();
            line.SetPosition(0, entrySpawns[i]);
            line.SetPosition(1, line2ndPointSpawns[i]);
            line.startColor = currColor;
            line.endColor = currColor;

            GameObject plug = Instantiate(wirePlug, shuffledExitSpawns[i], wirePlug.transform.rotation);
            SpriteRenderer plugSpriteRenderer = plug.GetComponent<SpriteRenderer>();
            plugSpriteRenderer.color = currColor;

            PlugStats plugStats = plug.GetComponent<PlugStats>();
            allPlugStats.Add(plugStats);
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
