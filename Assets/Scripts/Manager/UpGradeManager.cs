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
    // public TextMeshProUGUI healthLevelText;
    public TextMeshProUGUI staminaLevelText;
    public TextMeshProUGUI speedLevelText;
    public TextMeshProUGUI inventoryLevelText;
    public TextMeshProUGUI mapLevelText;
    public GameObject UpGradeGround;

    // (스탯, 텍스트, 콜백 함수)를 저장하는 리스트
    private List<(IntValueSO stat, TextMeshProUGUI text, Action<int> callback)> bindings = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // 업그레이드 할때마다 TextUI를 갱신해 주기 위해서 호출
        // TextUpdate(playerStats.healthLevel, healthLevelText);
        TextUpdate(playerStats.staminaLevel, staminaLevelText);
        TextUpdate(playerStats.speedLevel, speedLevelText);
        TextUpdate(playerStats.inventoryLevel, inventoryLevelText);
        TextUpdate(playerStats.mapLevel, mapLevelText);
    }

    void TextUpdate(IntValueSO Level, TextMeshProUGUI LevelText) // TextUI 갱신용 함수
    {
        Action<int> callback = newValue => 
        {
            LevelText.text = newValue.ToString();
            Debug.Log($"{LevelText.name}Level 바뀜: {LevelText.text}");
        };
        Level.Register(callback); // 변경 감지 등록
        LevelText.text = Level.Value.ToString(); // 초기 값 설정
        bindings.Add((Level, LevelText, callback)); // 해제할 수 있도록 저장
    }

    void OnDestroy()
    {
        foreach (var (stat, _, callback) in bindings)
        {
            stat.Unregister(callback); // 콜백 해제
        }
    }


    public void HealthUpGrade() // 체력 업그레이드
    {
        if (playerStats.healthLevel.Value < 10) {
            playerStats.healthLevel.Value += 1;
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