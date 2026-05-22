using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject clearUICanvas;

    void Awake()
    {
        Instance = this;
    }
    public void ShowClearUI()
    {
        if (clearUICanvas != null)
        {
            clearUICanvas.SetActive(true);
            Debug.Log("클리어 UI 표시");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
