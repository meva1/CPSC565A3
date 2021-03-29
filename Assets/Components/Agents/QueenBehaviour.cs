using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QueenBehaviour : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Antymology.Terrain.WorldManager wm;
    public int nestBlocksPlaced;
    Vector3 previousPosition;

    private void Awake()
    {
        nestBlocksPlaced = 0;
        wm = Antymology.Terrain.WorldManager.Instance;
        health = 1000;
        maxHealth = 1000;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        RandomMove(PossibleMoves());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == previousPosition)
        {
            Debug.Log("digging");
            Dig();
        }
        else
        {
            previousPosition = transform.position;
            LayNestBlock();
            //RandomMove(possibleMoves);
            AvoidHoleMove(PossibleMoves());
            Health();
            ConsumeMulch();
            DigIfTooHigh();
        }

    }

    void Health()
    {
        int x = (int)transform.position.x;
        int y = (int)(transform.position.y - 0.5f);
        int z = (int)transform.position.z;
        if (wm.GetBlock(x, y, z) is Antymology.Terrain.AcidicBlock)
        {
            health -= 2;
            //Debug.Log("Standing on acid");
        }
        else
        {
            health--;
        }

        if (health <= 0)
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

    void LayNestBlock()
    {
        int x = (int)transform.position.x;
        int y = (int)(transform.position.y-0.5f);
        int z = (int)transform.position.z;

        AbstractBlock nestBlock = new Antymology.Terrain.NestBlock();
        if(health > maxHealth/2)
        {
            wm.SetBlock(x, y+1, z, nestBlock);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            wm.surfaceBlocks[x, z]++;
            health = health/3;
            nestBlocksPlaced++;
            //Debug.Log("Nest block laid");

        }
        
    }

    void AvoidHoleMove(List<int[]> possibleMoves)
    {
        float groundHeight = transform.position.y - 0.5f;
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            if(groundHeight > wm.surfaceBlocks[possibleMoves[i][0]+1,possibleMoves[i][2]]
                && groundHeight > wm.surfaceBlocks[possibleMoves[i][0] - 1, possibleMoves[i][2]]
                && groundHeight > wm.surfaceBlocks[possibleMoves[i][0], possibleMoves[i][2]+1]
                && groundHeight > wm.surfaceBlocks[possibleMoves[i][0], possibleMoves[i][2] - 1])
            {
                possibleMoves.RemoveAt(i);
            }
        }
        if (possibleMoves.Count > 0)
        {
            
            RandomMove(possibleMoves);
        }
        else
        {
            Dig();
        }
        
    }

    void ConsumeMulch()
    {
        if (health < maxHealth/4)
        {
            int x = (int)transform.position.x;
            int y = (int)(transform.position.y - 0.5f);
            int z = (int)transform.position.z;
            bool soloOccupied = true;
            if (wm.GetBlock(x, y, z) is Antymology.Terrain.MulchBlock && x > 2 && y > 2 && z > 2
                && x < ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter - 2
                && y < ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter - 2
                && z < ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter - 2)
            {

                foreach (GameObject ant in wm.Ants)
                {
                    if (transform.position == ant.transform.position && !GameObject.ReferenceEquals(gameObject, ant))
                    {
                        soloOccupied = false;
                    }
                }
                if (soloOccupied)
                {
                    AbstractBlock newBlock = new Antymology.Terrain.AirBlock();
                    wm.SetBlock(x, y, z, newBlock);
                    transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                    wm.surfaceBlocks[x, z]--;
                    health = 1000;
                }
            }
        }
    }

    void DigIfTooHigh()
    {
        float groundHeight = transform.position.y - 0.5f;
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        if (wm.surfaceBlocks[x-1,z] + 2 < groundHeight && wm.surfaceBlocks[x + 1, z] + 2 < groundHeight && wm.surfaceBlocks[x, z-1] + 2 < groundHeight && wm.surfaceBlocks[x, z + 1] + 2 < groundHeight)
        {
            int y = (int)(transform.position.y - 0.5f);
            if (wm.GetBlock(x, y, z) is Antymology.Terrain.GrassBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.StoneBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.AcidicBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.NestBlock)
            {
                AbstractBlock newBlock = new Antymology.Terrain.AirBlock();
                wm.SetBlock(x, y, z, newBlock);
                transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                wm.surfaceBlocks[x, z]--;
            }
        }
    }

    void Dig()
    {
        Debug.Log("entering dig function:");

        int x = (int)transform.position.x;
        int y = (int)(transform.position.y - 0.5f);
        int z = (int)transform.position.z;
        if (wm.GetBlock(x, y, z) is Antymology.Terrain.GrassBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.StoneBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.AcidicBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.NestBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.MulchBlock)
        {
            AbstractBlock newBlock = new Antymology.Terrain.AirBlock();
            wm.SetBlock(x, y, z, newBlock);
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            wm.surfaceBlocks[x, z]--;
            Debug.Log("Successful dig");
        }

    }
}
