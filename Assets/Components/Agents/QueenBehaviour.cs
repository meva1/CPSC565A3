﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QueenBehaviour : MonoBehaviour
{
    public int health;
    public Antymology.Terrain.WorldManager wm;

    private void Awake()
    {
        wm = Antymology.Terrain.WorldManager.Instance;
        health = 1000;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<int[]> possibleMoves = PossibleMoves();
        RandomMove(possibleMoves);
    }

    void Health()
    {
        health--;
        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    List<int[]> PossibleMoves()
    {
        int[] coords;
        int x = (int)transform.position.x;
        int y = (int)(transform.position.y - 0.5f);
        int z = (int)transform.position.z;
        List<int[]> possibleMoves = new List<int[]>();
        if (Math.Abs(wm.surfaceBlocks[x - 1, z] - y) <= 2 && x - 1 > 0)
        {
            coords = new int[] { x - 1, wm.surfaceBlocks[x - 1, z], z };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x + 1, z] - y) <= 2 && x + 1 < (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter - 1))
        {
            coords = new int[] { x + 1, wm.surfaceBlocks[x + 1, z], z };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x, z - 1] - y) <= 2 && z - 1 > 0)
        {
            coords = new int[] { x, wm.surfaceBlocks[x, z - 1], z - 1 };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x, z + 1] - y) <= 2 && z + 1 < (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter - 1))
        {
            coords = new int[] { x, wm.surfaceBlocks[x, z + 1], z + 1 };
            possibleMoves.Add(coords);
        }


        return possibleMoves;
    }

    void RandomMove(List<int[]> possibleMoves)
    {
        int randMove;
        if (possibleMoves.Count > 0)
        {
            randMove = wm.RNG.Next(0, possibleMoves.Count);
            int[] newCoords = possibleMoves[randMove];
            transform.position = new Vector3((float)newCoords[0], (float)newCoords[1] + 0.5f, (float)newCoords[2]);
            
        }
    }
}
