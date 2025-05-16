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
    [SerializeField] private GameObject cellPrefab;
    [Serializable] public struct UpgradeBar
    {
        public Transform UpgradeBarPos;
        public int maxLevel;
    }
    [SerializeField] private UpgradeBar[] upgradeBars;
    [SerializeField] private List<List<UpgradeCellFill>> allUpgradeBars = new();





    private void Start()
    {
        foreach (var bar in upgradeBars)
        {
            var parentRect = bar.UpgradeBarPos.GetComponent<RectTransform>();
            float totalWidth = parentRect.rect.width;
            float cellWidth = totalWidth / bar.maxLevel;

            var cellList = new List<UpgradeCellFill>();

            for (int i = 0; i < bar.maxLevel; i++)
            {
                GameObject go = Instantiate(cellPrefab, bar.UpgradeBarPos);
                var controller = go.GetComponent<UpgradeCellFill>();

                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellWidth, rt.sizeDelta.y);

                controller.SetFillAmount(0f);
                controller.SetMaxWidth(cellWidth); // FillImage 최대 크기도 업데이트
                cellList.Add(controller);
            }
            allUpgradeBars.Add(cellList);
        }
    }


    public void HealthUpGrade(int level) // 체력 업그레이드
    {
        if (playerStats.healthLevel.Value < 10) {
            playerStats.healthLevel.Value += level;
            playerStats.health += 10;
        }
        
    }

    public void StaminaUpgrade()  // 스태미나 업그레이드
    {
        if (playerStats.staminaLevel.Value < 10) {
            playerStats.staminaLevel.Value += 1;
            playerStats.stamina += 10;
        }
        
    }
    
    public void SpeedUpgrade() // 이동 속도 업그레이드
    {
        if (playerStats.speedLevel.Value < 10) {
            playerStats.speedLevel.Value += 1;
            playerStats.speed += 1;
        }
        
    }

    public void InventoryUpgrade() // 가방 업그레이드
    {
        if (playerStats.inventoryLevel.Value < 10) {
            playerStats.inventoryLevel.Value += 1;
            inventory.inventorySlotCount.Value += 4;
        }
        
    }

    public void MapUpgrade() // 지도 업그레이드
    {
        if (playerStats.mapLevel.Value < 10) {
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