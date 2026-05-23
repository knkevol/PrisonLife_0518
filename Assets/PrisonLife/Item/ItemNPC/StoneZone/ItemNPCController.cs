using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemNPCController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 4f;

    [Header("Zone 연결")]
    public Transform stoneZoneCenter;
    public Transform beforeItemZoneCenter;  // BeforeItemZone 중심

    private NavMeshAgent agent;
    private bool isInStoneZone = false;
    private GameObject targetStone = null;

    public enum ItemNPCState
    {
        MoveToStoneZone,  // StoneZone으로 이동 중
        FindingStone,     // StoneZone 내부에서 돌 탐색
        ChasingStone,     // 돌 추적 중
    }

    private ItemNPCState currentState = ItemNPCState.MoveToStoneZone;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case ItemNPCState.MoveToStoneZone:
                CheckArrivedAtStoneZone();
                break;

            case ItemNPCState.FindingStone:
                Debug.LogError("Update___ItemNPCState.FindingStone:");
                FindAndChaseStone();
                break;

            case ItemNPCState.ChasingStone:
                CheckTargetStoneValid();
                CheckStoneOverlap();
                break;
        }
    }

    void CheckStoneOverlap()
    {
        // NPC 주변 돌 감지
        Collider[] hits = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Stone")) continue;

            PrisonStone stone = hit.GetComponent<PrisonStone>();
            if (stone == null || stone.isCollected) continue;

            // 돌 감지 → BeforeItemZone으로 이동
            StartCoroutine(SendStoneToBeforeItemZone(hit.gameObject));
            // 다음 돌 탐색
            targetStone = null;
            currentState = ItemNPCState.FindingStone;
            break;
        }
    }

    public void MoveToStoneZone()
    {
        if (agent == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        if (stoneZoneCenter == null)
        {
            Debug.LogError("stoneZoneCenter null");
            return;
        }

        bool result = agent.SetDestination(stoneZoneCenter.position);
        Debug.Log($"SetDestination 결과: {result} / 목적지: {stoneZoneCenter.position}");
    }

    public void StartFindingStone()
    {
        currentState = ItemNPCState.FindingStone;
        isInStoneZone = true;
        Debug.Log("돌 탐색 시작");
    }

    void CheckArrivedAtStoneZone()
    {
        if (agent.pathPending) return;
        if (agent.remainingDistance > 0.5f) return;

        // StoneZone 도착 → 탐색 시작
        StartFindingStone();
        Debug.Log("StoneZone 도착");
    }

    void FindAndChaseStone()
    {
        // 가장 가까운 돌 탐색
        GameObject nearestStone = FindNearestStone();

        if (nearestStone == null)
        {
            Debug.LogError("돌 nearestStone == null");
            //agent.SetDestination(stoneZoneCenter.position);
            return;
        }

        // 돌 발견 → 추적 시작
        targetStone = nearestStone;
        currentState = ItemNPCState.ChasingStone;
        agent.SetDestination(targetStone.transform.position);
        Debug.Log($"돌 발견 → 추적 시작: {targetStone.name}");
    }

    void CheckTargetStoneValid()
    {
        // 타겟 돌이 사라졌으면 다시 탐색
        if (targetStone == null || !targetStone.activeInHierarchy)
        {
            targetStone = null;
            currentState = ItemNPCState.FindingStone;
            return;
        }

        // 돌 위치 계속 업데이트
        agent.SetDestination(targetStone.transform.position);
    }

    GameObject FindNearestStone()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position,10f);

        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Debug.Log($"돌 FindNearestStone : {hit.gameObject.name}, {hit.gameObject}");
            if (!hit.CompareTag("Stone"))
            {
                Debug.Log($"돌 FindNearestStone 돌이랑 안부딪힘");
                continue;
            }

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.gameObject;
            }
        }

        return nearest;
    }

    IEnumerator SendStoneToBeforeItemZone(GameObject stone)
    {
        Debug.Log($"돌 SendStoneToBeforeItemZone");
        if (stone == null) yield break;

        stone.tag = "Untagged";

        // 현재 타겟이면 타겟 초기화
        if (targetStone == stone)
        {
            targetStone = null;
            currentState = ItemNPCState.FindingStone;
        }

        // BeforeItemZone으로 이동
        stone.transform.parent = beforeItemZoneCenter;

        // BeforeItemZone 내 쌓기 위치 계산
        int stoneCount = beforeItemZoneCenter.childCount - 1;
        stone.transform.position = beforeItemZoneCenter.position
            + new Vector3(0f, 0.3f * stoneCount, 0f);

        Collider col = stone.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        BeforeItemZone beforeZone = beforeItemZoneCenter.GetComponent<BeforeItemZone>();
        if (beforeZone != null)
        {
            beforeZone.AddCollectedStone(stone);
            StopCoroutine(WaitAndProcessStones());
            StartCoroutine(WaitAndProcessStones());
        }
        yield return null;
    }

    IEnumerator WaitAndProcessStones()
    {
        yield return new WaitForSeconds(0.3f);

        BeforeItemZone beforeZone = beforeItemZoneCenter.GetComponent<BeforeItemZone>();
        if (beforeZone != null)
        {
            Debug.Log($"ProcessStones 호출 → 돌 {beforeZone.GetStoneCount()}개");
            beforeZone.ProcessStones();
        }
    }

    public void OnEnterStoneZone()
    {
        isInStoneZone = true;
        Debug.Log("ItemNPC StoneZone 진입");
    }

    public void OnExitStoneZone()
    {
        isInStoneZone = false;
        Debug.Log("ItemNPC StoneZone 이탈");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
