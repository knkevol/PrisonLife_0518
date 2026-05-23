using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemZone : MonoBehaviour
{
    [Header("아이템 설정")]
    public List<ItemData> itemDataList;

    [Header("UI 설정")]
    public Canvas zoneCanvas;
    public TextMeshProUGUI priceText; 
    public Image itemIconImage;

    private Camera mainCamera;
    private bool isBuy = false;  // 구매 완료 여부
    private bool isAllBuy = false;  // 구매 완료 여부
    private int currentItemIndex = 0; // 구매 가능한 아이템 인덱스


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        // 처음에는 비활성화
        SetZoneActive(false);

        UpdateUI();
    }

    public void SetZoneActive(bool active)
    {
        if (zoneCanvas != null)
        {
            zoneCanvas.gameObject.SetActive(active);
        }

        // Collider 활성화/비활성화
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = active;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        // UI 카메라 바라보기
        if (zoneCanvas != null && mainCamera != null)
            zoneCanvas.transform.rotation = mainCamera.transform.rotation;
    }

    public int GetCurrentPrice()
    {
        if (itemDataList == null || itemDataList.Count == 0) return 0;
        if (currentItemIndex >= itemDataList.Count) return 0;
        return itemDataList[currentItemIndex].price;
    }
    void UpdateUI()
    {
        if (itemDataList == null || currentItemIndex >= itemDataList.Count) return;

        ItemData current = itemDataList[currentItemIndex];

        if (priceText != null)
        {
            priceText.text = $"${current.price}";
        }

        if (itemIconImage != null && current.itemIcon != null)
        {
            itemIconImage.sprite = current.itemIcon;
            itemIconImage.gameObject.SetActive(true);
        }
        else if (itemIconImage != null)
        {
            itemIconImage.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isAllBuy) return;
        if (!other.CompareTag("PrisonPlayer")) return;
        if (itemDataList == null || currentItemIndex >= itemDataList.Count) return;

        // 돈 확인
        ItemData currentItem = itemDataList[currentItemIndex];
        int currentMoney = MoneyManager.Instance.GetMoney();
        if (currentMoney < currentItem.price)
        {
            Debug.Log($"돈 부족 → 필요: {currentItem.price} / 보유: {currentMoney}");
            return;
        }

        // 구매 처리
        bool success = MoneyManager.Instance.SpendMoney(currentItem.price);
        if (!success) return;

        OnBuy(currentItem);
    }

    void OnBuy(ItemData item)
    {
        // 아이템 타입별 처리
        switch (item.itemType)
        {
            case ItemData.ItemType.Equipment:
                if (item.equipmentData != null)
                {
                    //EquipmentManager.Instance?.ShowEquipment();
                    EquipmentManager.Instance?.EquipItem(item.equipmentData);
                }
                break;

            case ItemData.ItemType.NPC:
                Debug.Log("NPC 케이스 진입");
                if (item.itemNPCData != null)
                {
                    SpawnItemNPC(item.itemNPCData);
                }
                else
                {
                    Debug.LogError("itemNPCData null");
                }
                break;
            case ItemData.ItemType.Clear:
                GameManager.Instance?.ShowClearUI();
                break;
        }
        Debug.Log("switch문 완료");
        currentItemIndex++;
        Debug.Log($"아이템인덱스++: {currentItemIndex} / 아이템리스트 갯수: {itemDataList.Count}");
        
        if (currentItemIndex >= itemDataList.Count)
        {
            Debug.Log($"currentItemIndex >= itemDataList.Count");
            isAllBuy = true;
            SetZoneActive(false);
        }
        else
        {
            UpdateUI();
        }
        ItemManager.Instance?.OnItemBuy(this);
    }
    public bool IsBuy() => isBuy;
    public bool IsAllBuy() => isAllBuy;

    public ItemData.ItemType GetCurrentItemType()
    {
        if (itemDataList == null || currentItemIndex >= itemDataList.Count)
        {
            return ItemData.ItemType.Equipment;
        }

        return itemDataList[currentItemIndex].itemType;
    }

    void SpawnItemNPC(ItemNPCData npcData)
    {
        // 스폰 위치는 NPCManager에서 관리
        ItemNPCManager.Instance?.SpawnItemNPC(npcData);
    }
}
