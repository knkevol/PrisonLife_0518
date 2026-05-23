using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCollider : MonoBehaviour
{

    private StoneCollector stoneCollector;
    private PrisonStone targetStone;

    // Start is called before the first frame update
    void Start()
    {
        stoneCollector = GetComponentInParent<StoneCollector>();
        if (stoneCollector == null)
        {
            Debug.LogError("StoneCollector 없음");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Stone")) return;

        PrisonStone stone = other.GetComponent<PrisonStone>();
        if (stone == null || stone.isCollected) return;

        Debug.Log($"장비에 Stone 감지: {other.gameObject.name}");
        CollectStone(stone);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Stone")) return;

        PrisonStone stone = other.GetComponent<PrisonStone>();
        if (stone == null || stone.isCollected) return;

        CollectStone(stone);
    }

    void CollectStone(PrisonStone stone)
    {
        if (stoneCollector == null) return;
        if (!stoneCollector.CanCollect()) return;

        stone.isCollected = true;

        // StoneCollector에 수집 요청
        stoneCollector.TryCollectStone(stone.gameObject);

        // Spawner에 수집 알림 → 리스폰 예약
        stone.spawner?.OnStoneCollected(stone.poolIndex);

        Debug.Log("Stone 수집 완료");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
