using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Stone 스폰 설정")]
    public GameObject stonePrefab;
    public float stoneSpacing = 7f;
    public float stoneYPosition = 0.5f;

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
            Debug.LogError("CalculateBounds()_StoneZone Renderer Error!");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        spawnerCenter = bounds.center;
        spawnerSize = bounds.size;

        Debug.Log($"StoneZone → Center: {spawnerCenter}, Size: {spawnerSize}");
    }

    void SpawnStones()
    {
        float startX = spawnerCenter.x - spawnerSize.x / 2f + stoneSpacing / 2f;
        float startZ = spawnerCenter.z - spawnerSize.z / 2f + stoneSpacing / 2f;
        float endX = spawnerCenter.x + spawnerSize.x / 2f;
        float endZ = spawnerCenter.z + spawnerSize.z / 2f;

        for (float x = startX; x < endX; x += stoneSpacing)
        {
            for (float z = startZ; z < endZ; z += stoneSpacing)
            {
                Vector3 spawnPos = new Vector3(x, spawnerCenter.y + stoneYPosition, z);
                GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);
                stone.tag = "Stone";
                stone.transform.parent = transform;
            }
        }
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
