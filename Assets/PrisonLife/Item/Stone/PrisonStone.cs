using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonStone : MonoBehaviour
{
    private StoneSpawner spawner;
    private int poolIndex;
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

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"PrisonStone OnTriggerStay: {other}");
        if (isCollected)
        {
            return;
        }

        if (other.CompareTag("PrisonPlayer"))
        {
            StoneCollector collector = other.GetComponent<StoneCollector>();
            if (collector != null)
            {
                isCollected = true;
                collector.TryCollectStone(gameObject);
                spawner?.OnStoneCollected(poolIndex);
            }
        }
    }
}
