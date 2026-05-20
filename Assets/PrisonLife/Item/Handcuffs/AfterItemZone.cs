using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterItemZone : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject itemPrefab;          // 스폰할 새 아이템 프리팹
    public Vector3 stackOffset = new Vector3(0f, 2f, 0f);  // 쌓기 간격
    public float handcuffsScale = 3f;           // 아이템 크기

    private List<GameObject> spawnedHandcuffs = new List<GameObject>();
    private bool isTransferring = false;

    // Start is called before the first frame update
    void Start()
    {
        itemPrefab = Resources.Load<GameObject>("PrisonHandcuffs");
        if (itemPrefab != null)
        {
            Debug.Log("PrisonHandcuffs Complete");
        }
        else
        {
            Debug.LogError("PrisonHandcuffs Null");
        }
    }

    public void SpawnItems(int count)
    {
        // 기존 아이템 제거
        ClearItems();

        Vector3 zoneCenter = GetZoneCenter();

        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(itemPrefab, zoneCenter, Quaternion.identity);
            item.transform.parent = transform;

            // Zone 중심 기준으로 쌓기
            Vector3 worldPos = zoneCenter + stackOffset * i;
            item.transform.position = worldPos;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one * handcuffsScale;

            // Collider는 유지 (플레이어 감지용)
            spawnedHandcuffs.Add(item);
        }

        Debug.Log($"AfterItemZone 아이템 {count}개 스폰 완료");
    }

    Vector3 GetZoneCenter()
    {
        Collider col = GetComponent<Collider>();
        return col != null ? col.bounds.center : transform.position;
    }

    void ClearItems()
    {
        //foreach (var item in spawnedHandcuffs)
        //    if (item != null) Destroy(item);

        spawnedHandcuffs.Clear();
        
        Debug.Log($"AfterItemZone 아이템 {spawnedHandcuffs.Count}개"); 
    }

    public int GetItemCount() => spawnedHandcuffs.Count;

    public List<GameObject> GetItems() => spawnedHandcuffs;

    public void ClearAllItems()
    {
        ClearItems();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
