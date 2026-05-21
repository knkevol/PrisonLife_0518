using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }
            
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;

            // 경유지 구 표시
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            // 경유지 간 선 연결
            if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }

    public List<Transform> GetWaypoints() => waypoints;
    // Update is called once per frame
    void Update()
    {
        
    }
}
