using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemNPCController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 10f;

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
        if (agent != null)
        {
            Debug.Log($"꼬우State: {currentState} / " +
                      $"isOnNavMesh: {agent.isOnNavMesh} / " +
                      $"pathPending: {agent.pathPending} / " +
                      $"remainingDistance: {agent.remainingDistance} / " +
                      $"pathStatus: {agent.pathStatus} / " +
                      $"destination: {agent.destination}");
        }

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
                break;
        }
    }

    public void MoveToStoneZone()
    {
        currentState = ItemNPCState.MoveToStoneZone;
        if (stoneZoneCenter == null)
        {
            Debug.LogError("StoneZoneCenter 없음");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"꼬우 NPC NavMesh 밖 → 현재위치: {transform.position}");
            return;
        }

        
        agent.SetDestination(stoneZoneCenter.position);

        bool result = agent.SetDestination(stoneZoneCenter.position);
        Debug.Log($"꼬우 SetDestination 결과: {result} / " +
              $"목적지: {stoneZoneCenter.position} / " +
              $"현재위치: {transform.position}");
    }

    public void StartFindingStone()
    {
        currentState = ItemNPCState.FindingStone;
        isInStoneZone = true;
        Debug.Log("돌 탐색 시작");
    }

    void CheckArrivedAtStoneZone()
    {
        Debug.Log($"꼬우 pathPending: {agent.pathPending} / " +
              $"remainingDistance: {agent.remainingDistance} / " +
              $"pathStatus: {agent.pathStatus}");

        if (agent.pathPending) return;
        if (agent.remainingDistance > 0.5f) return;

        // StoneZone 도착 → 탐색 시작
        StartFindingStone();
        Debug.Log("StoneZone 도착");
    }

    void FindAndChaseStone()
    {
        Debug.LogError("돌 FindAndChaseStone");
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
        Debug.LogError($"돌 FindAndChaseStone NPC CurrentState: {currentState}");
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"돌 OnTriggerEnter");
        Debug.Log($"돌 OnTriggerEnter {other.name}");
        if (!other.CompareTag("Stone")) return;
        if (!isInStoneZone) return;

        // 돌을 BeforeItemZone으로 이동
        StartCoroutine(SendStoneToBeforeItemZone(other.gameObject));
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

        Debug.Log($"돌 → BeforeItemZone으로 이동 완료");
        yield return null;
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
