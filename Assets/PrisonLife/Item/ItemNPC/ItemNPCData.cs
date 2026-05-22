using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemNPCData", menuName = "PrisonLife/ItemNPC Data")]
public class ItemNPCData : ScriptableObject
{
    public string npcName;     // 장비 이름
    public GameObject prefab;        // 장비 프리팹

    [Header("아이템 타입")]
    public ItemNPCType npcType;

    public enum ItemNPCType
    {
        StoneZone,  // 장비 (EquipmentData 연결)
        SellZone,        // NPC 아이템
    }
}

