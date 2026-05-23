using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MoneyZone : MonoBehaviour
{

    [Header("돈 설정")]
    public GameObject moneyPrefab;         // 지폐 프리팹
    public Vector3 moneyStackOffset = new Vector3(0f, 0.5f, 0f); // 쌓기 간격
    public float moneyScale = 18f;

    [Header("등 뒤 아이템")]
    public Vector3 moneyBackOffset = new Vector3(0f, 0.5f, -6f);   // 등 뒤 첫번째 위치

    private List<GameObject> moneyPrefabList = new List<GameObject>();
    private List<GameObject> playerMoneyList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        moneyPrefab = Resources.Load<GameObject>("PrisonMoney");
        if (moneyPrefab == null)
        {
            Debug.LogError("PrisonMoney Null");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PrisonPlayer"))
        {
            if (moneyPrefabList.Count == 0)
            {
                return;
            }

            MoneyManager.Instance?.AddMoney(moneyPrefabList.Count * 10);
            StartCoroutine(TransferMoneyToPlayer(other.transform));
        }
    }

    public void AddMoneyPrefab(int amount)
    {
        StartCoroutine(SpawnMoneyPrefabCoroutine(amount));
    }

    IEnumerator SpawnMoneyPrefabCoroutine(int amount)
    {
        Collider col = GetComponent<Collider>();
        Vector3 zoneCenter = col != null ? col.bounds.center : transform.position;

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = zoneCenter + moneyStackOffset * moneyPrefabList.Count;

            GameObject money = Instantiate(moneyPrefab, spawnPos, Quaternion.identity);
            money.transform.parent = transform;
            money.transform.localScale = Vector3.one * moneyScale;
            moneyPrefabList.Add(money);

            Debug.Log($"돈 추가 → 총 {moneyPrefabList.Count}장");

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator TransferMoneyToPlayer(Transform playerTransform)
    {
        int existingCount = GetExistingBackMoneyCount(playerTransform);

        for (int i = 0; i < moneyPrefabList.Count; i++)
        {
            GameObject money = moneyPrefabList[i];
            if (money == null)
            {
                Debug.Log($"money == null");
                continue;
            }

            // 플레이어 자식으로 편입
            money.transform.parent = playerTransform;

            // 등 뒤 위치 계산
            Vector3 localPos = moneyBackOffset + moneyStackOffset * (existingCount + i);
            money.transform.localPosition = localPos;
            money.transform.localRotation = Quaternion.identity;
            money.transform.localScale = Vector3.one * moneyScale;

            Collider col = money.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            playerMoneyList.Add(money);
            Debug.Log($"[{i}] 돈 이전 완료 → localPos: {localPos}");
            yield return new WaitForSeconds(0.05f);
        }
        moneyPrefabList.Clear();
        Debug.Log("돈 이전 완료");
    }

    public void RemoveMoneyPrefabs(int amount)
    {
        int removeCount = amount / 10;

        for (int i = 0; i < removeCount; i++)
        {
            if (playerMoneyList.Count == 0)
            {
                break;
            }

            // 마지막 머니부터 제거
            int lastIndex = playerMoneyList.Count - 1;
            GameObject money = playerMoneyList[lastIndex];
            playerMoneyList.RemoveAt(lastIndex);

            if (money != null)
            {
                Destroy(money);
            }
        }
    }

    int GetExistingBackMoneyCount(Transform playerTransform)
    {
        int count = 0;
        foreach (Transform child in playerTransform)
        {
            if (child.CompareTag("PrisonMoney"))
            {
                count++;
            }
        }
        return count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
