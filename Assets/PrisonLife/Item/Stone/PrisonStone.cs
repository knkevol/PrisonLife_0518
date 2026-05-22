using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonStone : MonoBehaviour
{
    public StoneSpawner spawner;
    public int poolIndex;
    public bool isCollected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(StoneSpawner stoneSpawner, int index)
    {
        spawner = stoneSpawner;
        poolIndex = index;
    }
}
