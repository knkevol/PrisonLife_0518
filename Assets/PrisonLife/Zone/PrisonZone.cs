using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrisonZone : MonoBehaviour
{
    public static PrisonZone Instance;

    [Header("인원 설정")]
    public int maxCapacity = 20;
    private int currentCount = 0;
    private int reservedCount = 0;  // 이동 중인 예약 인원

    [Header("UI 설정")]
    public Canvas prisonCanvas;              // World Space Canvas
    public GameObject capacityBubble;
    public TextMeshProUGUI capacityText;
    public Vector3 uiOffset = new Vector3(0f, 3f, 0f); // UI 높이

    private Camera mainCamera;
    private List<GameObject> prisonNPCs = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        // UI 항상 카메라 바라보기
        if (prisonCanvas != null && mainCamera != null)
        {
            // UI 위치 → PrisonZone 중심 위
            Collider col = GetComponent<Collider>();
            Vector3 zoneCenter = col != null ? col.bounds.center : transform.position;

            prisonCanvas.transform.position = zoneCenter + uiOffset;
            prisonCanvas.transform.rotation = mainCamera.transform.rotation;
        }
    }

    // NPC 이동 시작 시 호출 (SellCounter에서)
    public bool TryReserve()
    {
        if (currentCount + reservedCount >= maxCapacity)
        {
            Debug.Log("PrisonZone 가득 참 (예약 포함)");
            return false;
        }

        reservedCount++;
        Debug.Log($"PrisonZone 예약 → " +
                  $"현재: {currentCount} / " +
                  $"예약: {reservedCount} / " +
                  $"최대: {maxCapacity}");
        return true;
    }

    // ─── NPC 입장 ────────────────────────────────────
    public bool TryEnterPrison(GameObject npc)
    {
        if (reservedCount > 0)
        {
            reservedCount--;
        }

        prisonNPCs.Add(npc);
        currentCount++;
        UpdateUI();

        if (currentCount >= maxCapacity)
        {
            Debug.Log("PrisonZone 가득 참 → 입장 불가");
            NPCSpawner spawner = FindObjectOfType<NPCSpawner>();
            if (spawner != null)
            {
                spawner.ShowNoCellToAll();
            }
            return false;
        }

        Debug.Log($"NPC 입장 → {currentCount}/{maxCapacity}");
        return true;
    }

    // ─── UI 업데이트 ─────────────────────────────────
    void UpdateUI()
    {
        if (capacityText != null)
            capacityText.text = $"{currentCount}/{maxCapacity}";
    }

    // 예약 취소 (NoCell 등 이동 실패 시)
    public void CancelReserve()
    {
        if (reservedCount > 0)
        {
            reservedCount--;
        }

        Debug.Log($"예약 취소 → 예약: {reservedCount}");
    }

    public bool IsFull() => currentCount >= maxCapacity;
    public int GetCurrentCount() => currentCount;
}
