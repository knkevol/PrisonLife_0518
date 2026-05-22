using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemNPCController itemNPC = other.GetComponent<ItemNPCController>();
        if (itemNPC != null)
        {
            itemNPC.OnEnterStoneZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ItemNPCController itemNPC = other.GetComponent<ItemNPCController>();
        if (itemNPC != null)
        {
            itemNPC.OnExitStoneZone();
        }
    }
}
