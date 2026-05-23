using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Vector3 originLocalPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetOriginPosition(Vector3 pos)
    {
        originLocalPos = pos;
    }
}
