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
    public List<UpgradeBarMove> HealthUpgradeCells;
    public List<UpgradeBarMove> StaminaUpgradeCells;
    public List<UpgradeBarMove> SpeedUpgradeCells;
    public List<UpgradeBarMove> InventoryUpgradeCells;
    public List<UpgradeBarMove> MapUpgradeCells;


    public void HealthUpGrade(int level) // 체력 업그레이드
    {
        if (player.healthLevel.Value < player.healthLevel.maxLevel)
        {
            player.healthLevel.Value += level;
            player.healthLevel.max += 10;
            player.healthLevel.current += 10;

            TriggerCellMovement(HealthUpgradeCells, player.healthLevel.Value);
        }
    }

    public void StaminaUpgrade(int level)  // 스태미나 업그레이드
    {
        if (player.staminaLevel.Value < player.staminaLevel.maxLevel)
        {
            player.staminaLevel.Value += level;
            player.staminaLevel.max += 10;
            player.staminaLevel.current += 10;
            
            TriggerCellMovement(StaminaUpgradeCells, player.staminaLevel.Value);
        }
    }
    
    public void SpeedUpgrade(int level) // 이동 속도 업그레이드
    {
        if (player.speedLevel.Value < player.speedLevel.maxLevel)
        {
            player.speedLevel.Value += level;
            player.speedLevel.current += 1;

            TriggerCellMovement(SpeedUpgradeCells, player.speedLevel.Value);
        }
    }

    public void InventoryUpgrade(int level) // 가방 업그레이드
    {
        if (player.inventoryLevel.Value < player.inventoryLevel.maxLevel)
        {
            player.inventoryLevel.Value += level;
            inventory.inventorySlotCount.Value += inventory.inventorySlotCount.upgradeCount;
            
            TriggerCellMovement(InventoryUpgradeCells, player.inventoryLevel.Value);
        }
    }

    public void MapUpgrade(int level) // 지도 업그레이드
    {
        if (player.mapLevel.Value < player.mapLevel.maxLevel)
        {
            player.mapLevel.Value += level;
            
            TriggerCellMovement(MapUpgradeCells, player.mapLevel.Value);
        }
    }

    private void TriggerCellMovement(List<UpgradeBarMove> upgradeCells, int currentCellCount)
    {
        if (currentCellCount <= upgradeCells.Count)
        {
            upgradeCells[currentCellCount - 1].MoveToTarget();
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