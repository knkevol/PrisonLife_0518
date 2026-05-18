using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject stonePrefab;        // PrisonStone 프리팹 연결
    public float stoneSpacing = 1.5f;     // 돌 간격
    public float stoneYPosition = 0.5f;   // 돌 높이

    private BoxCollider zoneCollider;

    // Start is called before the first frame update
    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        SpawnStones();
    }

    void SpawnStones()
    {
        // StoneZone의 크기와 위치 가져오기
        Vector3 center = transform.position;
        Vector3 size = zoneCollider.size;

        float startX = center.x - size.x / 2f + stoneSpacing / 2f;
        float startZ = center.z - size.z / 2f + stoneSpacing / 2f;

        for (float x = startX; x < center.x + size.x / 2f; x += stoneSpacing)
        {
            for (float z = startZ; z < center.z + size.z / 2f; z += stoneSpacing)
            {
                Vector3 spawnPos = new Vector3(x, center.y + stoneYPosition, z);
                GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);
                stone.tag = "Stone"; // 태그 자동 설정
                stone.transform.parent = transform; // StoneZone 자식으로
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
