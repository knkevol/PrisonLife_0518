using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoneCollector : MonoBehaviour
{
    [Header("돌 설정")]
    public int maxStones = 20;
    private int currentStones = 0;

    [Header("돌 쌓이는 위치")]
    public Vector3 firstStoneOffset = new Vector3(0f, 0.5f, -5f);
    public Vector3 stackOffset = new Vector3(0f, 1f, 0f);
    public float stoneScale = 13f;

    [Header("MAX Text")]
    public GameObject maxCanvas;
    public TextMeshProUGUI maxText;
    public Vector3 maxTextOffset = new Vector3(0f, 2f, 0f);

    private Camera mainCamera;
    private bool isShowMax = false;
    private List<GameObject> collectedStones = new List<GameObject>();

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
        DetectNearbyStones();
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

        //Debug.Log($"DetectNearbyStones()");
        // 캐릭터 주변 반경 1f 안의 Stone 태그 오브젝트 감지
        // 언리얼의 SphereTraceByChannel과 동일한 개념
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Stone"))
            {
                Debug.Log($"DetectNearbyStones()");
                PrisonStone stone = hit.GetComponent<PrisonStone>();
                if (stone != null && !stone.isCollected)
                {
                    stone.isCollected = true;
                    TryCollectStone(hit.gameObject);
                }
            }
        }
    }

    public bool TryCollectStone(GameObject stone)
    {
        Debug.Log($"TryCollectStone()");
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
        stone.transform.localScale = Vector3.one * stoneScale;

        collectedStones.Add(stone);
        currentStones++;

        // 20개 도달 시 MAX 표시
        if (currentStones >= maxStones)
        {
            if (maxText != null)
                StartCoroutine(ShowMaxText());
        }

        Debug.Log($"돌 수집: {currentStones}/{maxStones}");
        return true;
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    public void ConsumeAllStones()
    {
        foreach (var stone in collectedStones)
        {
            if (stone != null)
                Destroy(stone);
        }
        collectedStones.Clear();
        currentStones = 0;

        if (maxText != null)
            maxText.gameObject.SetActive(false);
    }

}
