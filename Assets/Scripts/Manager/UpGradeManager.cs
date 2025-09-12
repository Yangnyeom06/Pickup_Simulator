using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpGradeManager : MonoBehaviour
{
    public static UpGradeManager Instance { get; private set; }
    public GameObject UpGradeGround;
    public List<UpgradeBarMove> HealthUpgradeCells;
    public List<UpgradeBarMove> StaminaUpgradeCells;
    public List<UpgradeBarMove> SpeedUpgradeCells;
    public List<UpgradeBarMove> InventoryUpgradeCells;
    public List<UpgradeBarMove> MapUpgradeCells;


    public void HealthUpGrade(int level) // 체력 업그레이드
    {
        if (PlayerManager.Instance.healthLevel.Value < PlayerManager.Instance.healthLevel.maxLevel)
        {
            PlayerManager.Instance.healthLevel.Value += level;
            PlayerManager.Instance.healthLevel.max += 10;
            PlayerManager.Instance.healthLevel.current += 10;

            TriggerCellMovement(HealthUpgradeCells, PlayerManager.Instance.healthLevel.Value);
        }
    }

    public void StaminaUpgrade(int level)  // 스태미나 업그레이드
    {
        if (PlayerManager.Instance.staminaLevel.Value < PlayerManager.Instance.staminaLevel.maxLevel)
        {
            PlayerManager.Instance.staminaLevel.Value += level;
            PlayerManager.Instance.staminaLevel.max += 10;
            PlayerManager.Instance.staminaLevel.current += 10;
            
            TriggerCellMovement(StaminaUpgradeCells, PlayerManager.Instance.staminaLevel.Value);
        }
    }
    
    public void SpeedUpgrade(int level) // 이동 속도 업그레이드
    {
        if (PlayerManager.Instance.speedLevel.Value < PlayerManager.Instance.speedLevel.maxLevel)
        {
            PlayerManager.Instance.speedLevel.Value += level;
            PlayerManager.Instance.speedLevel.current += 1;

            TriggerCellMovement(SpeedUpgradeCells, PlayerManager.Instance.speedLevel.Value);
        }
    }

    public void InventoryUpgrade(int level) // 가방 업그레이드
    {
        if (PlayerManager.Instance.inventoryLevel.Value < PlayerManager.Instance.inventoryLevel.maxLevel)
        {
            PlayerManager.Instance.inventoryLevel.Value += level;
            InventoryManager.Instance.inventorySlotCount.Value += InventoryManager.Instance.inventorySlotCount.upgradeCount;
            
            TriggerCellMovement(InventoryUpgradeCells, PlayerManager.Instance.inventoryLevel.Value);
        }
    }

    public void MapUpgrade(int level) // 지도 업그레이드
    {
        if (PlayerManager.Instance.mapLevel.Value < PlayerManager.Instance.mapLevel.maxLevel)
        {
            PlayerManager.Instance.mapLevel.Value += level;
            
            TriggerCellMovement(MapUpgradeCells, PlayerManager.Instance.mapLevel.Value);
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