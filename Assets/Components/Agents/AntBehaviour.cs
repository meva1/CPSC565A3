using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AntBehaviour : MonoBehaviour
{
    //private UnityEngine.XR.WSA.WorldManager worldManager;
    public int health;
    public float distanceToQueen;
    public Antymology.Terrain.WorldManager wm;

    

    private void Awake()
    {

        wm = Antymology.Terrain.WorldManager.Instance;
        health = 1000;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        distanceToQueen = Vector3.Distance(wm.queen.GetComponent<QueenBehaviour>().transform.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {

        List<int[]> possibleMoves = PossibleMoves();
        //RandomMove(possibleMoves);
        WeightedRandomMove(possibleMoves);
        ConsumeMulch();
        Health();
        DonateToQueen();
        distanceToQueen = Vector3.Distance(wm.queen.GetComponent<QueenBehaviour>().transform.position, transform.position);

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
            wm.Ants.Remove(gameObject);
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
        if(Math.Abs(wm.surfaceBlocks[x-1,z]-y) <= 2 && x - 1 > 0)
        {
            coords = new int[] { x - 1, wm.surfaceBlocks[x - 1, z], z };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x + 1, z] - y) <= 2 && x + 1 < (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter-1))
        {
            coords = new int[] { x + 1, wm.surfaceBlocks[x + 1, z], z };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x, z-1] - y) <= 2 && z - 1 > 0)
        {
            coords = new int[] { x, wm.surfaceBlocks[x, z-1], z-1 };
            possibleMoves.Add(coords);
        }
        if (Math.Abs(wm.surfaceBlocks[x, z + 1] - y) <= 2 && z + 1 < (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter-1))
        {
            coords = new int[] { x, wm.surfaceBlocks[x, z + 1], z + 1 };
            possibleMoves.Add(coords);
        }


        return possibleMoves;
    }

   void RandomMove(List<int[]> possibleMoves)
    {
        int randMove;
        if(possibleMoves.Count > 0)
        {
            randMove = wm.RNG.Next(0, possibleMoves.Count);
            int[] newCoords = possibleMoves[randMove];
            transform.position = new Vector3((float)newCoords[0], (float)newCoords[1]+0.5f, (float)newCoords[2]);
        }
    }

    void ConsumeMulch()
    {
        if (health < 250)
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

    void DonateToQueen()
    {
        if(transform.position == wm.queen.transform.position)
        {
            int donate = health * 3 / 4;
            health = health - donate;
            wm.queen.GetComponent<QueenBehaviour>().health += donate;
            if(wm.queen.GetComponent<QueenBehaviour>().health > 1000)
            {
                int returnHealth = wm.queen.GetComponent<QueenBehaviour>().health - 1000;
                health += returnHealth;
                wm.queen.GetComponent<QueenBehaviour>().health = 1000;
            }
            //Debug.Log("Donated to queen");
        }
    }

    void Dig()
    {
        int x = (int)transform.position.x;
        int y = (int)(transform.position.y - 0.5f);
        int z = (int)transform.position.z;
        if (wm.GetBlock(x, y, z) is Antymology.Terrain.GrassBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.StoneBlock || wm.GetBlock(x, y, z) is Antymology.Terrain.AcidicBlock)
        {
            AbstractBlock newBlock = new Antymology.Terrain.AirBlock();
            wm.SetBlock(x, y, z, newBlock);
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            wm.surfaceBlocks[x, z]--;
        }
    }

    void WeightedRandomMove(List<int[]> possibleMoves)
    {
        double p = wm.RNG.NextDouble();
        double[] moveWeights = new double[possibleMoves.Count];
        float currentDistanceToQueen = Vector3.Distance(wm.queen.GetComponent<QueenBehaviour>().transform.position, transform.position);
        float possibleDistanceToQueen;
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            moveWeights[i] = 1;
        }
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            possibleDistanceToQueen = Vector3.Distance(wm.queen.GetComponent<QueenBehaviour>().transform.position, new Vector3((float)possibleMoves[i][0], (float)possibleMoves[i][1], (float)possibleMoves[i][2]));
            if(possibleDistanceToQueen < currentDistanceToQueen)
            {
                moveWeights[i] += 2;
            }
        }
        double weightSum = 0;
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            weightSum += moveWeights[i];
        }
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            moveWeights[i] = moveWeights[i] / weightSum;
        }
        double prevWeights = 0;
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            if (p < (moveWeights[i] + prevWeights))
            {
                int[] newCoords = possibleMoves[i];
                transform.position = new Vector3((float)newCoords[0], (float)newCoords[1] + 0.5f, (float)newCoords[2]);
                return;
            }
            prevWeights += moveWeights[i];
        }

    }

    void GetDigWeight(List<int[]> possibleMoves)
    {
        int digWeight = 0;
        int lowerCount = 0;
        int higherCount = 0;
        foreach(int[] move in possibleMoves)
        {
            if((int)(transform.position.y-0.5f) < move[1])
            {
                higherCount++;
            }
            if ((int)(transform.position.y - 0.5f) > move[1])
            {
                lowerCount++;
            }
        }
    }



}
