using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 여기서는 사고 파는것만 구현, 각각 상점의 로직은 따로
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public PlayerStats playerStats;
    [SerializeField] private List<TextMeshProUGUI> moneyTexts = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddMoney(int amount)
    {
        playerStats.money += amount;
        UpdateMoneyUI();
    }

    public void SpendMoney(int amount)
    {
        if (playerStats.money >= amount)
        {
            playerStats.money -= amount;
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }

    private void UpdateMoneyUI()
    {
        foreach (var text in moneyTexts)
        {
            text.text = playerStats.money.ToString();
        }
    }
    
}
