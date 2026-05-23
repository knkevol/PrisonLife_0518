using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("장비 장착 위치")]
    public Transform equipSlot;       // 장비가 붙을 위치 (캐릭터 손/허리 등)

    [Header("기본 장비")]
    public EquipmentData defaultEquipment;    // 기본 장비

    [Header("기본 설정")]
    public float equipScale = 3f;

    private GameObject currentEquipObj;       // 현재 장착된 장비 오브젝트
    private Equipment currentEquipment;       // 현재 장비 컴포넌트
    private bool isVisible = false;           // 장비 표시 여부

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (defaultEquipment != null)
            EquipItem(defaultEquipment);
    }

    public void EquipItem(EquipmentData data)
    {
        // 기존 장비 제거
        if (currentEquipObj != null)
        {
            Destroy(currentEquipObj);
        }

        currentEquipObj = Instantiate(data.prefab, equipSlot);
        currentEquipObj.transform.localPosition = data.positionOffset;
        currentEquipObj.transform.localRotation = Quaternion.Euler(data.rotationOffset);
        currentEquipObj.transform.localScale = data.scaleOffset;

        currentEquipment = currentEquipObj.GetComponent<Equipment>();

        if (currentEquipment != null)
        {
            currentEquipment.SetOriginPosition(data.positionOffset);
        }

        StoneCollector stoneCollector = GetComponentInParent<StoneCollector>();
        if (stoneCollector != null)
        {
            stoneCollector.SetEquipmentTransform(
            currentEquipObj.transform,
            data.overlapSize  // 장비마다 다른 크기 전달
        );
        }

        currentEquipObj.SetActive(isVisible);

        Debug.Log($"장비 장착: {data.equipmentName}");
    }

    public void ShowEquipment()
    {
        isVisible = true;
        if (currentEquipObj != null)
            currentEquipObj.SetActive(true);

        Debug.Log("장비 표시");
    }

    public void HideEquipment()
    {
        isVisible = false;
        if (currentEquipObj != null)
            currentEquipObj.SetActive(false);

        Debug.Log("장비 숨김");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
