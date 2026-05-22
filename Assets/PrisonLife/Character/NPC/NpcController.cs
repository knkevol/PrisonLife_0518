using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("구매 설정")]
    public int desiredAmount;

    [Header("말풍선 UI")]
    public Canvas bubbleCanvas;
    public GameObject bubble;
    public TextMeshProUGUI bubbleText;

    [Header("PrisonZone 이동")]
    public float moveSpeed = 10f;
    public Transform prisonZoneTarget; // PrisonZone 입장 위치
    private Vector3 targetPosition; // NPC가 이동해야 할 목표 위치(대기열)

    [Header("경유지 설정")]
    public List<Transform> waypoints;  // 경유지 목록
    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    private Camera mainCamera;
    private NPCSpawner spawner;        // 소속 Spawner 참조

    public enum NPCState
    {
        Waiting,    // SellZone 대기
        Moving,     // PrisonZone 이동중
        Arrived,
        NoCell,      // PrisonZone 가득 참
    }

    public NPCState currentState = NPCState.Waiting;

    void Start()
    {
        mainCamera = Camera.main;
        desiredAmount = Random.Range(1, 6);
        UpdateBubbleText(desiredAmount.ToString());
    }
    private void Update()
    {
        Debug.Log($"isMoving?: {isMoving}");
        Debug.Log($"currentState?: {currentState}");
        if (!isMoving && currentState == NPCState.Waiting)
        {
            transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
            );
        }

        MoveAlongWaypoints();
    }

    void MoveAlongWaypoints()
    {
        Debug.Log($"currentWaypointIndex: {currentWaypointIndex}");
        Debug.Log($"waypoints.Count: {waypoints.Count}");
        if (currentWaypointIndex >= waypoints.Count)
        {
            isMoving = false;
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        if(target == null)
        {
            return;
        }

        // 현재 경유지로 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // 이동 방향으로 회전
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // 현재 경유지 도착 확인
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            // 마지막 경유지 도착
            if (currentWaypointIndex >= waypoints.Count)
            {
                isMoving = false;
                currentState = NPCState.Arrived;
                OnArrivedAtPrisonZone();
                Debug.Log("최종 목적지 도착");
            }
        }
    }

    // NPCSpawner에서 호출 → Spawner 참조 전달
    public void Initialize(NPCSpawner npcSpawner)
    {
        spawner = npcSpawner;
        targetPosition = transform.position; // 기본 위치
    }

    public void SetTargetPosition(Vector3 newTargetPos)
    {
        targetPosition = newTargetPos;
    }

    void LateUpdate()
    {
        if (bubbleCanvas != null && mainCamera != null)
            bubbleCanvas.transform.rotation = mainCamera.transform.rotation;

        // PrisonZone으로 이동 중일 때
        //if (currentState == NPCState.Moving && prisonZoneTarget != null)
        //{
        //    MovePrisonZone();
        //}
    }

    void MovePrisonZone()
    {
        Debug.Log("MovePrisonZone");
        Vector3 targetPos = prisonZoneTarget.position;
        SetTargetPosition(targetPos);

        // 목적지 도착 확인
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            OnArrivedAtPrisonZone();
            return;
        }
    }

    void OnArrivedAtPrisonZone()
    {
        Debug.Log("OnArrivedAtPrisonZone");
        isMoving = false;
        waypoints = null;
        currentState = NPCState.Arrived;

        if (PrisonZone.Instance != null)
        {
            bool entered = PrisonZone.Instance.TryEnterPrison(gameObject);

            if (!entered)
            {
                // 입장 실패 → No Cell 표시
                ShowNoCell();
                return;
            }
        }

        spawner = null;
        transform.parent = PrisonZone.Instance?.transform; //PrisonZOne 자식으로

        // 말풍선 숨기기
        if (bubbleCanvas != null)
            bubbleCanvas.gameObject.SetActive(false);
    }

    //NPC 상태변경
    public void StartMoveToPrison(List<Transform> targetWaypoints)
    {
        Debug.Log("StartMoveToPrison");
        waypoints = targetWaypoints;
        currentWaypointIndex = 0;
        isMoving = true;
        currentState = NPCState.Moving;
        bubbleCanvas.gameObject.SetActive(false);
    }

    public void ShowNoCell()
    {
        currentState = NPCState.NoCell;
        UpdateBubbleText("No Cell");
    }

    public void UpdateBubbleText(string text)
    {
        if (bubbleText != null)
        {
            bubbleText.text = text;
            bubbleText.fontSize = 0.8f;
        }
    }
}