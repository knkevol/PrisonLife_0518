using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("ItemZone 목록")]
    public List<ItemZone> itemZones;   // Inspector에서 등록

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        itemZones.Sort((a, b) => a.GetCurrentPrice().CompareTo(b.GetCurrentPrice()));

        // 초기 상태 체크
        CheckAndActivateZones();
    }

    public void OnMoneyChanged(int currentMoney)
    {
        CheckAndActivateZones();
    }

    void CheckAndActivateZones()
    {
        int currentMoney = MoneyManager.Instance.GetMoney();

        foreach (ItemZone zone in itemZones)
        {
            if (zone == null) continue;
            if (zone.IsBuy()) continue;

            // 보유 금액 >= 아이템 가격이면 활성화
            if (currentMoney >= zone.GetCurrentPrice() && zone.GetCurrentPrice() > 0)
            {
                zone.SetZoneActive(true);
                Debug.Log($"Zone 활성화 → 현재 아이템 가격: {zone.GetCurrentPrice()}");
            }
            else
            {
                zone.SetZoneActive(false);
            }
        }
    }

    public void OnItemBuy(ItemZone purchasedZone)
    {
        CheckAndActivateZones();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
