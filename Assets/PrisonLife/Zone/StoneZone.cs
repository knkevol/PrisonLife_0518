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
        Debug.Log($"돌 → StoneZone ______OnTriggerEnter");
        ItemNPCController itemNPC = other.GetComponent<ItemNPCController>();
        Debug.Log($"돌 → StoneZone ______OnTriggerEnter {itemNPC}");
        if (itemNPC != null)
        {
            itemNPC.OnEnterStoneZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"돌 → StoneZone OnTriggerExit");
        ItemNPCController itemNPC = other.GetComponent<ItemNPCController>();
        Debug.Log($"돌 → StoneZone OnTriggerExit {itemNPC}");
        if (itemNPC != null)
        {
            itemNPC.OnExitStoneZone();
        }
    }
}
