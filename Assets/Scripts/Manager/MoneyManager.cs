using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 여기서는 사고 파는것만 구현, 각각 상점의 로직은 따로
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
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
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.money += amount;
        }
        
        if (DayManager.Instance != null)
        {
            DayManager.Instance.todayGetMoney += amount;
        }
        else
        {
            Debug.LogWarning("DayManager가 할당되지 않았습니다!");
        }
        
        UpdateMoneyUI();
    }

    public void SpendMoney(int amount)
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.money >= amount)
        {
            PlayerManager.Instance.money -= amount;
            
            if (DayManager.Instance != null)
            {
                DayManager.Instance.todaySpendMoney -= amount;
            }
            else
            {
                Debug.LogWarning("DayManager가 할당되지 않았습니다!");
            }
            
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }

    public void UpdateMoneyUI()
    {
        if (PlayerManager.Instance != null)
        {
            foreach (var text in moneyTexts)
            {
                if (text != null)
                {
                    text.text = PlayerManager.Instance.money.ToString();
                }
            }
        }
    }
    
}
