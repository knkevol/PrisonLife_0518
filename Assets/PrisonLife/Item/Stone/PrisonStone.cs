using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonStone : MonoBehaviour
{

    public bool isCollected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCollected) return; // 이미 수집됐으면 무시

        Debug.Log($"OnTriggerStay: {other.gameObject.name}");

        if (other.CompareTag("PrisonPlayer"))
        {
            StoneCollector collector = other.GetComponent<StoneCollector>();
            if (collector != null)
            {
                isCollected = true; // 먼저 플래그 세움 (중복 방지)
                collector.TryCollectStone(gameObject);
            }
        }
    }
}
