using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpGradeManager : MonoBehaviour
{
    public static UpGradeManager Instance { get; private set; }
    public PlayerStats playerStats;
    public SaveManager saveManager;
    public InventoryManager inventory;
    public GameObject UpGradeGround;



    private void Start()
    {
        
    }


    public void HealthUpGrade(int level) // 체력 업그레이드
    {
        if (playerStats.healthLevel.Value < playerStats.healthLevel.maxLevel)
        {
            playerStats.healthLevel.Value += level;
            playerStats.healthLevel.max += 10;
            playerStats.healthLevel.current += 10;
        }
        
    }

    public void StaminaUpgrade()  // 스태미나 업그레이드
    {
        if (playerStats.staminaLevel.Value < playerStats.staminaLevel.maxLevel)
        {
            playerStats.staminaLevel.Value += 1;
            playerStats.staminaLevel.max += 10;
            playerStats.staminaLevel.current += 10;
        }
        
    }
    
    public void SpeedUpgrade() // 이동 속도 업그레이드
    {
        if (playerStats.speedLevel.Value < playerStats.speedLevel.maxLevel)
        {
            playerStats.speedLevel.Value += 1;
            playerStats.speedLevel.current += 1;
        }
        
    }

    public void InventoryUpgrade() // 가방 업그레이드
    {
        if (playerStats.inventoryLevel.Value < playerStats.inventoryLevel.maxLevel) {
            playerStats.inventoryLevel.Value += 1;
            inventory.inventorySlotCount.Value += inventory.inventorySlotCount.upgradeCount;
        }
        
    }

    public void MapUpgrade() // 지도 업그레이드
    {
        if (playerStats.mapLevel.Value < playerStats.mapLevel.maxLevel) {
            playerStats.mapLevel.Value += 1;
        }
        
    }

    public void Open() // UpGradeUI 열기
    {
        UpGradeGround.SetActive(true);
    }

    public void Exit() // UpGradeUI 닫기
    {
        UpGradeGround.SetActive(false);
    }
}