using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrisonZone : MonoBehaviour
{
    public static PrisonZone Instance;

    [Header("인원 설정")]
    public int maxCapacity = 10;
    private int currentCount = 0;

    [Header("UI 설정")]
    public Canvas prisonCanvas;              // World Space Canvas
    public GameObject capacityBubble;
    public TextMeshProUGUI capacityText;     // 0/10 텍스트
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

    // ─── NPC 입장 ────────────────────────────────────
    public bool TryEnterPrison(GameObject npc)
    {
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

    public bool IsFull() => currentCount >= maxCapacity;
    public int GetCurrentCount() => currentCount;
}
