using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterItemZone : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject handcuffsPrefab;
    public Vector3 stackOffset = new Vector3(0f, 1f, 0f);
    public float handcuffsScale = 6f;

    private List<GameObject> spawnedHandcuffs = new List<GameObject>();
    private bool isTransferring = false;

    // Start is called before the first frame update
    void Start()
    {
        handcuffsPrefab = Resources.Load<GameObject>("PrisonHandcuffs");
        if (handcuffsPrefab == null)
        {
            Debug.LogError("PrisonHandcuffs Null");
        }
    }

    public void SpawnItems(int count)
    {
        // 기존 아이템 제거
        //ClearItems();
        StartCoroutine(SpawnItemsCoroutine(count));
    }

    IEnumerator SpawnItemsCoroutine(int count)
    {
        Collider col = GetComponent<Collider>();
        Vector3 zoneCenter = GetZoneCenter();

        for (int i = 0; i < count; i++)
        {
            // 기존 아이템 수 기준으로 위치 계산
            Vector3 spawnPos = zoneCenter + stackOffset * spawnedHandcuffs.Count;

            GameObject item = Instantiate(handcuffsPrefab, spawnPos, Quaternion.identity);
            item.transform.parent = transform;
            item.transform.localScale = Vector3.one * handcuffsScale;
            spawnedHandcuffs.Add(item);

            Debug.Log($"아이템 추가 → 총 {spawnedHandcuffs.Count}개");
            yield return new WaitForSeconds(0.05f);
        }
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
