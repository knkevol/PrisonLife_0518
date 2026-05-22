using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeItemZone : MonoBehaviour
{
    [Header("AfterItemZone")]
    public AfterItemZone afterItemZone;

    private List<GameObject> collectedStones = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCollectedStone(GameObject stone)
    {
        collectedStones.Add(stone);

        Debug.Log($"BeforeItemZone 돌 추가: {collectedStones.Count}개");
    }

    public void ProcessStones()
    {
        if (collectedStones.Count == 0)
        {
            return;
        }

        int count = collectedStones.Count;

        // AfterItemZone에 스폰
        if (afterItemZone != null)
        {
            afterItemZone.SpawnItems(count);
        }

        ClearStones();
    }
    void ClearStones()
    {
        foreach (var stone in collectedStones)
        {
            if (stone != null)
                Destroy(stone);
        }
        collectedStones.Clear();
        Debug.Log("BeforeItemZone 돌 전체 삭제 완료");
    }

    public int GetStoneCount() => collectedStones.Count;
}
