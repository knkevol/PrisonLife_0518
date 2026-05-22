using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class StoneCollector : MonoBehaviour
{
    [Header("돌 설정")]
    public int maxStones = 10;
    private int currentStones = 0;

    [Header("뒤쪽 아이템 쌓기 설정")]
    public Vector3 firstStoneOffset = new Vector3(0f, 0.5f, -3f);
    public Vector3 stackOffset = new Vector3(0f, 2f, 0f);
    public Vector3 stoneScale = new Vector3(20f, 10f, 20f);

    [Header("앞쪽 아이템 쌓기 설정")]
    public Vector3 handcuffsOffset = new Vector3(2f, 1f, 5f);   // 첫번째 아이템 위치 (앞쪽)
    public Vector3 handcuffsStackOffset = new Vector3(0f, 1f, 0f);    // 쌓기 간격
    public float handcuffsScale = 6f;

    [Header("MAX Text")]
    public GameObject maxCanvas;
    public TextMeshProUGUI maxText;
    public Vector3 maxTextOffset = new Vector3(0f, 2f, 0f);

    [Header("BeforeItemZone")]
    public Vector3 zoneStackOffset = new Vector3(0f, 2f, 0f);
    public float zoneStoneScale = 8f;

    [Header("AfterItemZone")]
    public AfterItemZone afterItemZone;

    private Camera mainCamera;
    private bool isShowMax = false;
    private bool isTransferring = false; // 중복 이전 방지
    private List<GameObject> collectedStones = new List<GameObject>();
    private List<GameObject> collectedHandcuffs = new List<GameObject>();

    private bool isInStoneZone = false; // StoneZone 진입 감지

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if(maxCanvas != null)
        {
            maxCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isInStoneZone)
        {
            DetectNearbyStones();
        }
    }

    private void LateUpdate()
    {
        if (maxCanvas != null && maxCanvas.activeSelf)
        {
            // 플레이어 위치 추적
            maxCanvas.transform.position = transform.position + maxTextOffset;

            // 카메라 방향
            if (mainCamera != null)
                maxCanvas.transform.rotation = mainCamera.transform.rotation;
        }
    }

    void DetectNearbyStones()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        bool stoneNearby = false;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Stone"))
            {
                continue;
            }

            PrisonStone stone = hit.GetComponent<PrisonStone>();
            if (stone == null || stone.isCollected) continue;

            stone.isCollected = true;
            stoneNearby = true;
            TryCollectStone(hit.gameObject);
            stone.spawner?.OnStoneCollected(stone.poolIndex);

        }

        if (stoneNearby)
        {
            EquipmentManager.Instance?.StartEuqipMoving();
        }
        else
        {
            EquipmentManager.Instance?.StopEuqipMoving();
        }
    }

    public bool TryCollectStone(GameObject stone)
    {
        if (currentStones >= maxStones)
        {
            return false; // 수정예정 :: 수집 but max 띄우기
        }

        // 돌을 캐릭터 등 뒤로 이동
        stone.tag = "Untagged"; // 중복 수집 방지
        stone.transform.parent = transform; // 캐릭터 자식으로

        // 콜라이더 제거 (더 이상 충돌 불필요)
        Collider col = stone.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 등 뒤 위치 계산
        Vector3 localPos = firstStoneOffset + stackOffset * currentStones;
        stone.transform.localPosition = localPos;
        stone.transform.localRotation = Quaternion.identity;
        stone.transform.localScale = stoneScale;

        collectedStones.Add(stone);
        currentStones++;

        if (currentStones >= maxStones)
        {
            if (maxText != null)
                StartCoroutine(ShowMaxText());
        }

        Debug.Log($"돌 수집: {currentStones}/{maxStones}");
        return true;
    }

    public bool CanCollect() => currentStones < maxStones;

    IEnumerator ShowMaxText()
    {
        isShowMax = true;
        if (maxCanvas != null)
        {
            maxCanvas.SetActive(true);
        }
            
        yield return new WaitForSeconds(3f);

        if (maxCanvas != null)
        {
            maxCanvas.SetActive(false);
        }


        isShowMax = false;
    }

    public int GetStoneCount() => currentStones;
    public int GetFrontItemCount() => collectedHandcuffs.Count;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[트리거] 부딪힌 물체 이름: {other.gameObject.name} | 태그: {other.tag}");

        Transform beforeZone = GetZoneTransform(other.transform, "BeforeItemZone");
        if (beforeZone != null && !isTransferring)
        {
            if (collectedStones.Count == 0)
            {
                Debug.Log("collectedStones.Count == 0");
                return;
            }

            StartCoroutine(TransferStonesToZone(beforeZone));
        }

        Transform afterZone = GetZoneTransform(other.transform, "AfterItemZone");
        if (afterZone != null && !isTransferring)
        {
            AfterItemZone zone = afterZone.GetComponent<AfterItemZone>();
            if (zone != null && zone.GetItemCount() > 0)
            {
                StartCoroutine(TransferItemsToFront(zone));
            }
            return;
        }

        Transform counterZone = GetZoneTransform(other.transform, "SellCounter");
        if (counterZone != null && !isTransferring)
        {
            if (collectedHandcuffs.Count == 0)
            {
                Debug.Log("handcuffs.Count == 0");
                return;
            }

            StartCoroutine(TransferHandcuffsToZone(counterZone));
        }

        Transform stoneZone = GetZoneTransform(other.transform, "StoneZone");
        //if (other.CompareTag("StoneZone"))
        if (stoneZone)
        {
            isInStoneZone = true;
            EquipmentManager.Instance?.ShowEquipment();
            Debug.Log("StoneZone 진입 → 장비 표시");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform stoneZone = GetZoneTransform(other.transform, "StoneZone");
        //if (other.CompareTag("StoneZone"))
        if(stoneZone)
        {
            isInStoneZone = false;
            EquipmentManager.Instance?.HideEquipment();
        }
    }

    IEnumerator TransferHandcuffsToZone(Transform zoneTransform)
    {
        isTransferring = true;

        SellCounter counter = zoneTransform.GetComponent<SellCounter>();

        for (int i = 0; i < collectedHandcuffs.Count; i++)
        {
            GameObject handcuffs = collectedHandcuffs[i];

            if (handcuffs == null)
                continue;

            handcuffs.transform.parent = zoneTransform; // SellCounter를 부모로 지정

            Vector3 localPos = handcuffsStackOffset + (handcuffsStackOffset * i);
            handcuffs.transform.localPosition = localPos;
            handcuffs.transform.localRotation = Quaternion.identity;
            handcuffs.transform.localScale = Vector3.one * handcuffsScale;
            
            Collider col = handcuffs.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            yield return new WaitForSeconds(0.05f);
        }

        isTransferring = false;
        counter.AddHandcuffs(collectedHandcuffs);
        ClearHandcuffs();
        counter.TryBuy();
    }

    IEnumerator TransferItemsToFront(AfterItemZone zone)
    {
        isTransferring = true;

        List<GameObject> items = new List<GameObject>(zone.GetItems());
        //Debug.Log($"이전할 아이템 수: {items.Count}");

        for (int i = 0; i < items.Count; i++)
        {
            GameObject item = items[i];
            if (item == null)
            {
                continue;
            }

            //Debug.Log($"[{i}] 이전 전 부모: {item.transform.parent?.name}");
            item.transform.parent = transform; // 플레이어로 부모 변경
            //Debug.Log($"[{i}] 부모 변경 후: {item.transform.parent?.name}");

            Vector3 localPos = handcuffsOffset + handcuffsStackOffset * (collectedHandcuffs.Count);
            item.transform.localPosition = localPos;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one * handcuffsScale;

            //Debug.Log($"[{i}] localPosition: {item.transform.localPosition}");
            //Debug.Log($"[{i}] worldPosition: {item.transform.position}");
            //Debug.Log($"worldScale: {item.transform.lossyScale}");

            Collider col = item.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            collectedHandcuffs.Add(item);

            yield return new WaitForSeconds(0.05f);
        }
        
        // AfterItemZone 비우기
        zone.ClearAllItems();
        isTransferring = false;
    }

    Transform GetZoneTransform(Transform target, string tag) //부모 탐색해서 원하는 태그 찾기
    {
        Transform current = target;

        while (current != null)
        {
            //Debug.Log($"[태그 탐색] {current.name} | 태그: {current.tag}");

            if (current.CompareTag(tag))
                return current;

            current = current.parent;
        }

        return null;
    }

    IEnumerator TransferStonesToZone(Transform zoneTransform)
    {
        isTransferring = true;

        //Debug.Log($"돌 {collectedStones.Count}개 이전 시작");

        int stoneCount = collectedStones.Count;

        for (int i = 0; i < collectedStones.Count; i++)
        {
            GameObject stone = collectedStones[i];

            if (stone == null) 
                continue;

            // BeforeItemZone을 부모로 설정
            stone.transform.parent = zoneTransform;

            Vector3 localPos = zoneStackOffset * i;
            stone.transform.localPosition = localPos;
            stone.transform.localRotation = Quaternion.identity;
            stone.transform.localScale = Vector3.one * zoneStoneScale;

            // Collider 다시 비활성화 유지
            Collider col = stone.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            yield return new WaitForSeconds(0.05f);
        }

        // MAX UI 숨기기
        if (maxCanvas != null)
            maxCanvas.SetActive(false);

        isTransferring = false;

        if (afterItemZone != null)
        {
            afterItemZone.SpawnItems(stoneCount);
            //Debug.Log($"AfterItemZone 스폰 요청: {stoneCount}개");
        }

        // 리스트 초기화
        ClearStones();
        currentStones = 0;
    }

    void ClearStones()
    {
        foreach (var item in collectedStones)
            if (item != null) Destroy(item);

        collectedStones.Clear();
    }

    void ClearHandcuffs()
    {
        //foreach (var item in collectedHandcuffs)
        //    if (item != null) Destroy(item);

        collectedHandcuffs.Clear();
    }

}
