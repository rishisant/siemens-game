using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public System.Random rand = new System.Random();

    public GameObject sourcePrefab;
    public GameObject sinkPrefab;
    public GameObject straightPipe;
    public GameObject turnPipe;

    // initialize explicitly
    private PipeInfo[,] level = {

    };

    // Start is called before the first frame update
    void Start()
    {
        // have 3 categories of levels, easy, medium, hard
        // maybe have 3 levels for each, so 3 easy, 3 medium, 3 hard
        // randomly pick an easy level, then randomly pick a medium level, then randomly pick a hard level
        // each level is a nxn matrix with - meaning straight pipe, l meaning turnpipe(starting position of these is in an l), s meaning source (start), and e meaning sink (end)
        // we will generate the level, and then when use colliders to see when 2 pipes are connected
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
