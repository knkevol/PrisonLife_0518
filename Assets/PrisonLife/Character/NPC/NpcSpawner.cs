using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject npcPrefab;
    public int maxNPCCount = 5;
    public float npcSpacing = 20f;
    public float npcYPosition = 1f;
    public float npcScale = 2f;
    public Vector3 spawnStartPos;

    public enum SpawnAxis { X, Z }
    public SpawnAxis spawnAxis = SpawnAxis.X;

    // 현재 SellZone에 있는 NPC 목록
    private List<GameObject> activeNPCs = new List<GameObject>();

    private Vector3 npcSpawnerCenter;
    private Vector3 npcSpawnerSize;

    void Start()
    {
        CalculateBounds();
        FillNPC();
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

        npcSpawnerCenter = bounds.center;
        npcSpawnerSize = bounds.size;

        
    }

    private void OnDrawGizmos()
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    // NPC가 5명이 될때까지 채우는 함수
    // 구매완료 트리거에서 이 함수 호출
    public void FillNPC()
    {
        int currentCount = GetActiveNPCCount();
        int needToSpawn = maxNPCCount - currentCount;

        Debug.Log($"현재 NPC 수: {currentCount} / 추가 스폰: {needToSpawn}");

        for (int i = 0; i < needToSpawn; i++)
        {
            SpawnNPC();
        }
    }

    // 실제 활성화된 NPC 수 체크 (null 제거 포함)
    int GetActiveNPCCount()
    {
        // PrisonZone으로 이동한 NPC는 null이 아니라
        // activeNPCs에서 제거되어야 하므로
        // NPCController에서 OnNPCLeave 호출 필요
        activeNPCs.RemoveAll(npc => npc == null);
        return activeNPCs.Count;
    }

    void SpawnNPC()
    {
        int index = activeNPCs.Count;

        float startX = npcSpawnerCenter.x - npcSpawnerSize.x / 2f + npcSpacing / 2f;
        float endX = npcSpawnerCenter.x + npcSpawnerSize.x / 2f;

        // 이번 NPC가 배치될X 좌표
        float targetX = startX + (index * npcSpacing);

        if (targetX >= endX)
        {
            Debug.LogWarning("Not SellZone");
            return;
        }

        Vector3 spawnPos = new Vector3(targetX, npcSpawnerCenter.y + npcYPosition, npcSpawnerCenter.z);

        // 프리팹 생성 및 초기화
        GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
        npc.transform.parent = transform;
        npc.transform.position = spawnPos;
        npc.transform.localScale = Vector3.one * npcScale;

        // NPCController 컴포넌트 초기화
        NPCController controller = npc.GetComponent<NPCController>();
        if (controller != null)
        {
            controller.Initialize(this);
        }

        activeNPCs.Add(npc);
        //Debug.Log($"함수 끝 직전 실좌표: {npc.transform.position}, 실스케일: {npc.transform.localScale}");
    }

    // NPCController에서 PrisonZone 이동 시 호출
    public void OnNPCLeave(GameObject npc)
    {
        if (activeNPCs.Contains(npc))
        {
            activeNPCs.Remove(npc);
            Debug.Log($"NPC SellZone 이탈 → 현재 NPC 수: {activeNPCs.Count}");
        }

        // NPC 떠난 후 자리 채우기
        FillNPC();
    }

    
}