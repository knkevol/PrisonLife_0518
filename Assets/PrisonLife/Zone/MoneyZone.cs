using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyZone : MonoBehaviour
{

    [Header("µ· ¼³Á¤")]
    public GameObject moneyPrefab;         // ÁöÆó ÇÁ¸®ÆÕ
    public Vector3 moneyStackOffset = new Vector3(0f, 0.5f, 0f); // ½×±â °£°Ý
    public float moneyScale = 18f;

    private List<GameObject> moneyPrefabList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        moneyPrefab = Resources.Load<GameObject>("PrisonMoney");
        if (moneyPrefab == null)
        {
            Debug.LogError("PrisonMoney Null");
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

            // MoneyManager UI ¾÷µ¥ÀÌÆ®
            //MoneyManager.Instance?.AddMoney(10);

            Debug.Log($"µ· Ãß°¡ ¡æ ÃÑ {moneyPrefabList.Count}Àå");

            yield return new WaitForSeconds(0.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
