using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "PrisonLife/Equipment Data")]
public class EquipmentData : ScriptableObject
{
    public string equipmentName;     // 장비 이름
    public GameObject prefab;        // 장비 프리팹
    public bool isDefault = false;   // 기본 장비 여부

    [Header("장착 위치 설정")]
    public Vector3 positionOffset = Vector3.zero;   // 위치 오프셋
    public Vector3 rotationOffset = Vector3.zero;   // 회전 오프셋
    public Vector3 scaleOffset = Vector3.one;        // 크기 오프셋
}
