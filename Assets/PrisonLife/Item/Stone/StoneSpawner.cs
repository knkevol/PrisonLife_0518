using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [Header("Stone 스폰 설정")]
    public GameObject stonePrefab;
    public float stoneSpacing = 5f;
    public float stoneYPosition = 0.5f;
    public float respawnDelay = 3f;

    private List<Vector3> spawnPositions = new List<Vector3>();

    // 풀링: 위치 인덱스 → 돌 오브젝트
    private Dictionary<int, GameObject> stonePool = new Dictionary<int, GameObject>();


    private Vector3 spawnerCenter;
    private Vector3 spawnerSize;

    void Start()
    {
        CalculateBounds();
        SpawnStones();
    }

    void CalculateBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        float startX = bounds.min.x + stoneSpacing / 2f;
        float startZ = bounds.min.z + stoneSpacing / 2f;

        for (float x = startX; x < bounds.max.x; x += stoneSpacing)
        {
            for (float z = startZ; z < bounds.max.z; z += stoneSpacing)
            {
                Vector3 pos = new Vector3(x, bounds.center.y + stoneYPosition, z);
                spawnPositions.Add(pos);
            }
        }

        Debug.Log($"StoneZone → Center: {spawnerCenter}, Size: {spawnerSize}");
    }

    void SpawnStones()
    {
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject stone = Instantiate(stonePrefab, spawnPositions[i], Quaternion.identity);
            stone.transform.parent = transform;
            stone.tag = "Stone";

            // PrisonStone에 인덱스와 Spawner 참조 전달
            PrisonStone stoneScript = stone.GetComponent<PrisonStone>();
            if (stoneScript != null)
                stoneScript.Initialize(this, i);

            stonePool[i] = stone;
        }
    }

    public void OnStoneCollected(int index)
    {
        if (!stonePool.ContainsKey(index))
        {
            return;
        }

        stonePool.Remove(index); // 수집된 돌은 플레이어의 자식으로 들어가기 때문에 분리 필요

        // 딜레이 후 리스폰
        StartCoroutine(RespawnStone(index));
    }

    IEnumerator RespawnStone(int index)
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject stone = Instantiate(stonePrefab, spawnPositions[index], Quaternion.identity);
        stone.transform.parent = transform;
        stone.tag = "Stone";

        PrisonStone stoneScript = stone.GetComponent<PrisonStone>();
        if (stoneScript != null)
        {
            stoneScript.Initialize(this, index);
        }

        // 풀에 새 돌 등록
        stonePool[index] = stone;

        Debug.Log($"돌 {index} 리스폰");
    }


    private void OnDrawGizmos()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("OnDrawGizmos()_StoneZone Renderer Error!");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
