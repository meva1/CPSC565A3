using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public int nestBlocks;
    public Text nestText;
    // Start is called before the first frame update
    void Start()
    {
        nestText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        nestText.text = "Nest blocks placed: " + Antymology.Terrain.WorldManager.Instance.nestBlocks;
    }
}
