using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpGradeManager : MonoBehaviour
{
    public static UpGradeManager Instance { get; private set; }
    public PlayerManager player;
    public SaveManager saveManager;
    public InventoryManager inventory;
    public GameObject UpGradeGround;



    private void Start()
    {
        
    }


    public void HealthUpGrade(int level) // 체력 업그레이드
    {
        if (player.healthLevel.Value < player.healthLevel.maxLevel)
        {
            player.healthLevel.Value += level;
            player.healthLevel.max += 10;
            player.healthLevel.current += 10;
        }
    }

    public void StaminaUpgrade()  // 스태미나 업그레이드
    {
        if (player.staminaLevel.Value < player.staminaLevel.maxLevel)
        {
            player.staminaLevel.Value += 1;
            player.staminaLevel.max += 10;
            player.staminaLevel.current += 10;
        }
    }
    
    public void SpeedUpgrade() // 이동 속도 업그레이드
    {
        if (player.speedLevel.Value < player.speedLevel.maxLevel)
        {
            player.speedLevel.Value += 1;
            player.speedLevel.current += 1;
        }
    }

    public void InventoryUpgrade() // 가방 업그레이드
    {
        if (player.inventoryLevel.Value < player.inventoryLevel.maxLevel) {
            player.inventoryLevel.Value += 1;
            inventory.inventorySlotCount.Value += inventory.inventorySlotCount.upgradeCount;
        }
    }

    public void MapUpgrade() // 지도 업그레이드
    {
        if (player.mapLevel.Value < player.mapLevel.maxLevel) {
            player.mapLevel.Value += 1;
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