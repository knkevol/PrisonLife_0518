using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItemNPCController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 10f;
    public int maxCarryCount = 10;
    public float arriveDistance = 0.5f; // 도착 판정 거리

    [Header("Zone 연결")]
    public AfterItemZone afterItemZone;     // AfterItemZone 컴포넌트
    public SellCounter sellCounter;          // SellCounter 컴포넌트

    private Transform afterItemZoneCenter;
    private Transform sellCounterCenter;

    [Header("아이템 설정")]
    public Vector3 stackOffset = new Vector3(0f, 0.2f, 0f);
    public Vector3 counterStackOffset = new Vector3(0f, 1f, 0f);

    private List<GameObject> carriedItems = new List<GameObject>();
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector3 spawnPosition;

    public enum SellNPCState
    {
        MoveToAfterZone,
        CollectItems,
        MoveToSellCounter,
        DropItems,
        Waiting
    }

    private SellNPCState currentState = SellNPCState.Waiting;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;

        afterItemZone = FindObjectOfType<AfterItemZone>();
        sellCounter = FindObjectOfType<SellCounter>();

        afterItemZoneCenter = afterItemZone.transform;
        sellCounterCenter = sellCounter.transform;

        StartCoroutine(NPCRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;

        // 목적지로 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 이동 방향으로 회전
        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    IEnumerator MoveTo(Transform target)
    {
        targetPosition = new Vector3(
            target.position.x,
            transform.position.y, // Y값 고정
            target.position.z
        );
        isMoving = true;

        // 도착할 때까지 대기
        yield return new WaitUntil(() =>
            Vector3.Distance(transform.position, targetPosition) < arriveDistance
        );

        isMoving = false;
        Debug.Log($"{target.name} 도착");
    }

    IEnumerator NPCRoutine()
    {
        while (true)
        {
            // AfterItemZone 아이템 없으면 대기
            if (afterItemZone == null || afterItemZone.GetItemCount() == 0)
            {
                currentState = SellNPCState.Waiting;
                Debug.Log("아이템 없음 → 대기");
                yield return new WaitForSeconds(1f);
                continue;
            }

            // 1. AfterItemZone으로 이동
            currentState = SellNPCState.MoveToAfterZone;
            yield return StartCoroutine(MoveTo(afterItemZoneCenter));

            // 2. 아이템 수집
            yield return StartCoroutine(CollectItems());

            if (carriedItems.Count == 0)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            // 3. SellCounter로 이동
            currentState = SellNPCState.MoveToSellCounter;
            yield return StartCoroutine(MoveTo(sellCounterCenter));

            // 4. 아이템 내려놓기
            yield return StartCoroutine(DropItems());
        }
    }

    IEnumerator CollectItems()
    {
        currentState = SellNPCState.CollectItems;

        if (afterItemZone == null) yield break;

        List<GameObject> items = new List<GameObject>(afterItemZone.GetItems());
        int collectCount = Mathf.Min(items.Count, maxCarryCount);

        for (int i = 0; i < collectCount; i++)
        {
            GameObject item = items[i];
            if (item == null) continue;

            item.transform.parent = transform;
            item.transform.localPosition = stackOffset * i;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
            Collider col = item.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            carriedItems.Add(item);
            yield return new WaitForSeconds(0.05f);
        }

        //afterZone.RemoveItems(collectCount);
        afterItemZone.ClearAllItems();
        Debug.Log($"아이템 수집 완료: {carriedItems.Count}개");
    }

    IEnumerator DropItems()
    {
        Debug.Log("DropItems");
        currentState = SellNPCState.DropItems;

        if (sellCounter != null)
        {
            sellCounter.AddHandcuffs(carriedItems);
        }

        for (int i = 0; i < carriedItems.Count; i++)
        {
            GameObject item = carriedItems[i];
            if (item == null) continue;

            // NPC 자식에서 SellCounter 자식으로 이전
            item.transform.parent = sellCounterCenter;

            // SellCounter 내 위치 설정
            item.transform.localPosition = counterStackOffset * i;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(0.05f);
        }

        carriedItems.Clear();
        Debug.Log("아이템 내려놓기 완료");

        yield return new WaitForSeconds(0.3f);

        sellCounter.TryBuy();

        // 4. 스폰 위치로 복귀
        yield return StartCoroutine(MoveToSpawnPoint());
    }

    IEnumerator MoveToSpawnPoint()
    {
        Debug.Log("스폰 위치로 복귀");

        while (Vector3.Distance(transform.position, spawnPosition) > arriveDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                spawnPosition,
                moveSpeed * Time.deltaTime
            );

            Vector3 dir = (spawnPosition - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            yield return null;
        }

        Debug.Log("스폰 위치 복귀 완료");
    }
}
