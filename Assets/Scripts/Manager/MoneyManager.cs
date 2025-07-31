using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 여기서는 사고 파는것만 구현, 각각 상점의 로직은 따로
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public PlayerManager player;
    public asdfManager asdf;
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
        if (player != null)
        {
            player.money += amount;
        }
        
        if (asdf != null)
        {
            asdf.todayGetMoney += amount;
        }
        else
        {
            Debug.LogWarning("asdfManager가 할당되지 않았습니다!");
        }
        
        UpdateMoneyUI();
    }

    public void SpendMoney(int amount)
    {
        if (player != null && player.money >= amount)
        {
            player.money -= amount;
            
            if (asdf != null)
            {
                asdf.todaySpendMoney -= amount;
            }
            else
            {
                Debug.LogWarning("asdfManager가 할당되지 않았습니다!");
            }
            
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }

    private void UpdateMoneyUI()
    {
        if (player != null)
        {
            foreach (var text in moneyTexts)
            {
                if (text != null)
                {
                    text.text = player.money.ToString();
                }
            }
        }
    }
    
}
