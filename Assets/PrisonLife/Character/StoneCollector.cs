using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoneCollector : MonoBehaviour
{
    [Header("돌 설정")]
    public int maxStones = 20;
    private int currentStones = 0;
    public Vector3 firstStoneOffset = new Vector3(0f, 0.5f, -0.8f);  // 첫 번째 돌 위치 (등 뒤)
    public Vector3 stackOffset = new Vector3(0f, 0.4f, 0f);           // 돌 쌓이는 간격
    public float stoneScale = 10f;                                    // 등에 붙는 돌 크기

    [Header("MAX Text")]
    public TextMeshPro maxText; // 캐릭터에 띄울 MAX 텍스트

    private List<GameObject> collectedStones = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (maxText != null)
            maxText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DetectNearbyStones();
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
            return false;

        // 돌을 캐릭터 등 뒤로 이동
        stone.tag = "Untagged"; // 중복 수집 방지
        stone.transform.parent = transform; // 캐릭터 자식으로

        // 콜라이더 제거 (더 이상 충돌 불필요)
        Collider col = stone.GetComponent<Collider>();
        if (col != null) col.enabled = false;

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
        maxText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        maxText.gameObject.SetActive(false);
    }

    public int GetStoneCount() => currentStones;

    // 4단계 제련기에서 사용
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
