using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "PrisonLife/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName;
    public int price;                  // 아이템 가격
    public Sprite itemIcon;            // 아이템 아이콘 (UI용)

    [Header("아이템 타입")]
    public ItemType itemType;

    public enum ItemType
    {
        Equipment,  // 장비 (EquipmentData 연결)
        NPC,        // NPC 아이템
        Clear       // Clear
    }

    [Header("장비 아이템일 경우")]
    public EquipmentData equipmentData; // 장비 데이터 연결

    [Header("NPC 아이템일 경우")]
    public ItemNPCData itemNPCData; // NPC 데이터 연결
}