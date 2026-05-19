using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{

    public static MoneyManager Instance;

    [Header("Money")]
    public TextMeshProUGUI moneyText;

    private int currentMoney = 0;

    void Awake()
    {
        // == ¾ð¸®¾ó GameInstance
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney < amount) return false;
        currentMoney -= amount;
        UpdateMoneyUI();
        return true;
    }

    void UpdateMoneyUI()
    {
        moneyText.text = currentMoney.ToString();
    }

    public int GetMoney() => currentMoney;

    // Update is called once per frame
    void Update()
    {
        
    }

}
