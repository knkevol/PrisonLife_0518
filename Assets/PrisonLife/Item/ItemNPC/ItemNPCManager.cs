using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNPCManager : MonoBehaviour
{
    public static ItemNPCManager Instance;

    [Header("StoneZone NPC 스폰 위치")]
    public Transform stoneZoneSpawnPoint;   // StoneZone 입구
    public Transform stoneZoneCenter;       // StoneZone 중심
    public Transform beforeItemZoneCenter;  // BeforeItemZone 중심

    [Header("SellZone NPC 스폰 위치")]
    public Transform sellZoneSpawnPoint;    // SellZone 입구

    // 현재 활성화된 NPC 목록
    private List<GameObject> activeStoneNPCs = new List<GameObject>();
    private List<GameObject> activeSellNPCs = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public void SpawnItemNPC(ItemNPCData npcData)
    {
        switch (npcData.npcType)
        {
            case ItemNPCData.ItemNPCType.StoneZone:
                SpawnStoneZoneNPC(npcData);
                break;

            case ItemNPCData.ItemNPCType.SellZone:
                SpawnSellZoneNPC(npcData);
                break;
        }
    }

    void SpawnStoneZoneNPC(ItemNPCData npcData)
    {
        if (stoneZoneSpawnPoint == null)
        {
            Debug.LogError("StoneZone 스폰 위치 없음");
            return;
        }

        GameObject npc = Instantiate(
            npcData.prefab,
            stoneZoneSpawnPoint.position,
            Quaternion.identity
        );

        // ItemNPCController에 Zone 정보 전달
        ItemNPCController controller = npc.GetComponent<ItemNPCController>();
        if (controller != null)
        {
            controller.stoneZoneCenter = stoneZoneCenter;
            controller.beforeItemZoneCenter = beforeItemZoneCenter;
            controller.MoveToStoneZone();
        }

        activeStoneNPCs.Add(npc);
        Debug.Log($"StoneZone NPC 스폰: {npcData.npcName}");
    }

    void SpawnSellZoneNPC(ItemNPCData npcData)
    {
        if (sellZoneSpawnPoint == null)
        {
            Debug.LogError("SellZone 스폰 위치 없음");
            return;
        }

        GameObject npc = Instantiate(
            npcData.prefab,
            sellZoneSpawnPoint.position,
            Quaternion.Euler(0f, 180f, 0f)
        );

        activeSellNPCs.Add(npc);
        Debug.Log($"SellZone NPC 스폰: {npcData.npcName}");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
