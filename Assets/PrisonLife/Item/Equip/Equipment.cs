using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [Header("장비 움직임 설정")]
    public float equipSpeed = 20f;       // 움직임 속도
    public float equipMove = 20f;    // 움직임 폭

    private Vector3 originLocalPos;
    private bool isEquipMoving = false;   // 움직임 활성화 여부
    private float equipMovingTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(isEquipMoving)
        {
            equipMovingTimer += Time.deltaTime * equipSpeed;
            float newY = originLocalPos.y + Mathf.Sin(equipMovingTimer) * equipMove;
            transform.localPosition = new Vector3(
                originLocalPos.x,
                newY,
                originLocalPos.z
            );
        }
        else
        {
            // 멈추면 원래 위치로 복귀
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originLocalPos,
                Time.deltaTime * 5f
            );
            equipMovingTimer = 0f;
        }
    }

    public void SetOriginPosition(Vector3 pos)
    {
        originLocalPos = pos;
    }

    public void StartEuqipMoving() => isEquipMoving = true;
    public void StopEuqipMoving() => isEquipMoving = false;
}
