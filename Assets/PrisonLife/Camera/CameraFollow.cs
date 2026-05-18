using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public float height = 35f;    // 높이
    public float zOffset = 20f;   // 뒤로 얼마나 당길지
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(
            target.position.x,
            target.position.y + height,
            target.position.z - zOffset
        );
    }
}
