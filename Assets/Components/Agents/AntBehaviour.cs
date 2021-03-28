using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehaviour : MonoBehaviour
{

    public int health;

    private void Awake()
    {

        health = 1000;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Health();
        
    }

    void Health()
    {
        health--;
        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
