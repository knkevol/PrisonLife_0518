using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellCounter : MonoBehaviour
{
    public static SellCounter Instance;

    [Header("수갑 설정")]
    private List<GameObject> handcuffsList = new List<GameObject>();

    [Header("MoneyZone 연결")]
    public MoneyZone moneyZone;

    [Header("NPCSpawner 연결")]
    public NPCSpawner npcSpawner;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (npcSpawner == null)
            npcSpawner = FindObjectOfType<NPCSpawner>();

        if (moneyZone == null)
            moneyZone = FindObjectOfType<MoneyZone>();
    }

    public void AddHandcuffs(List<GameObject> handcuffs)
    {
        handcuffsList.AddRange(handcuffs);
        Debug.Log($"SellCounter 수갑 추가 → 총 {handcuffsList.Count}개");
    }

    public int GetHandcuffsCount() => handcuffsList.Count;

    public List<GameObject> RemoveHandcuffs(int amount)
    {
        List<GameObject> removed = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            if (handcuffsList.Count == 0) break;

            // 마지막 수갑부터 제거
            int lastIndex = handcuffsList.Count - 1;
            GameObject handcuff = handcuffsList[lastIndex];
            removed.Add(handcuff);
            handcuffsList.RemoveAt(lastIndex);

            if (handcuff != null)
                Destroy(handcuff);
        }

        Debug.Log($"수갑 {amount}개 차감 → 남은 수갑: {handcuffsList.Count}개");
        return removed;
    }

    public void TryBuy()
    {
        if (npcSpawner == null || handcuffsList.Count == 0)
        {
            Debug.Log("NPCSpawner X or Handcuffs X");
            return;
        }

        // 가장 우측 NPC 가져오기
        NPCController rightNPC = npcSpawner.GetRightmostNPC();

        if (rightNPC == null)
        {
            Debug.Log("NPC 없음");
            return;
        }

        if (rightNPC.currentState != NPCController.NPCState.Waiting) return;

        int desired = rightNPC.desiredAmount;
        int available = handcuffsList.Count;

        Debug.Log($"구매 시도 → NPC 요청: {desired}개 / 보유: {available}개");

        if (available >= desired)
        {
            // 구매 성공
            RemoveHandcuffs(desired);

            // MoneyZone에 돈 쌓기
            if (moneyZone != null)
                moneyZone.AddMoneyPrefab(desired);

            // NPC Moving 상태로 변경
            rightNPC.StartMoveToPrison(npcSpawner.GetPrisonTarget());

            // NPCSpawner에 이탈 알림
            npcSpawner.OnNPCLeave(rightNPC.gameObject);

            Debug.Log($"구매 완료 → {desired}개 / 돈 {desired * 10}원 지급");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
