using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum SnackEffectType
{
    Stamina = 0,            // 스태미나 증가
    Health = 1,             // 체력 증가
    Both = 2,               // 체력과 스태미나 동시 증가
    HealthLossReduction = 3, // 체력 소모량 감소 버프
    StaminaLossReduction = 4, // 스테미나 소모량 감소 버프
    RunSpeedBoost = 5       // 달리기 속도 증가 버프
}

public enum StoreItemDataType
{
    ShopItem,
    Snack
}

[CreateAssetMenu(fileName = "NewStoreItem", menuName = "Store/Store Item Data")]
public class StoreItemData : ScriptableObject
{
    [Header("공통 데이터")]
    public string itemID;               // 고유 식별자
    public string itemName;             // 아이템 이름 (상점: itemName, 간식: snackName)
    public Sprite icon;                 // 아이콘 이미지
    public int price;                   // 가격
    public string description;          // 설명
    public int slotNum;                 // 슬롯 번호
    
    [Header("아이템 타입")]
    public StoreItemDataType dataType = StoreItemDataType.ShopItem;
    
    [Header("상점 아이템 전용 데이터")]
    [SerializeField] private ItemType _itemType;           // 물티슈 등 (상점 아이템용)
    [SerializeField] private string _itemCode;             // 내부 식별용 코드 (예: "drink_cola")
    
    [Header("간식 전용 데이터")]
    [SerializeField] private SnackEffectType _effectType = SnackEffectType.Stamina; // 효과 타입
    [SerializeField] private float _itemStat;              // 간식을 먹으면 스테미나 또는 체력 증가(10~30) / 버프 강도(퍼센트)
    [SerializeField] private int _shelfLifeDays = 5;       // 유통기한 (기본 5일)
    [SerializeField] private float _buffDuration = 60f;    // 버프 지속시간 (초, 버프 타입일 경우에만 사용)

    // 상점 아이템 속성들 (ShopItemData 호환)
    public ItemType itemType 
    { 
        get => dataType == StoreItemDataType.ShopItem ? _itemType : ItemType.Small; 
        set => _itemType = value; 
    }
    
    public string itemCode 
    { 
        get => dataType == StoreItemDataType.ShopItem ? _itemCode : ""; 
        set => _itemCode = value; 
    }

    // 간식 속성들 (SnackItemData 호환)
    public string snackName 
    { 
        get => dataType == StoreItemDataType.Snack ? itemName : ""; 
        set { if (dataType == StoreItemDataType.Snack) itemName = value; }
    }
    
    public SnackEffectType effectType 
    { 
        get => dataType == StoreItemDataType.Snack ? _effectType : SnackEffectType.Stamina; 
        set => _effectType = value; 
    }
    
    public float itemStat 
    { 
        get => dataType == StoreItemDataType.Snack ? _itemStat : 0f; 
        set => _itemStat = value; 
    }
    
    public int shelfLifeDays 
    { 
        get => dataType == StoreItemDataType.Snack ? _shelfLifeDays : 0; 
        set => _shelfLifeDays = value; 
    }
    
    public float buffDuration 
    { 
        get => dataType == StoreItemDataType.Snack ? _buffDuration : 0f; 
        set => _buffDuration = value; 
    }

    // 간식 관련 메서드들 (SnackItemData와 동일)
    /// <summary>
    /// 구매일로부터 며칠이 지났는지 계산
    /// </summary>
    /// <param name="purchaseDay">구매한 날짜 (총 경과 일수)</param>
    /// <param name="currentDay">현재 날짜 (총 경과 일수)</param>
    /// <returns>경과 일수</returns>
    public int GetDaysElapsed(int purchaseDay, int currentDay)
    {
        if (dataType != StoreItemDataType.Snack) return 0;
        return currentDay - purchaseDay;
    }
    
    /// <summary>
    /// 간식이 만료되었는지 확인
    /// </summary>
    /// <param name="purchaseDay">구매한 날짜</param>
    /// <param name="currentDay">현재 날짜</param>
    /// <returns>만료되었으면 true</returns>
    public bool IsExpired(int purchaseDay, int currentDay)
    {
        if (dataType != StoreItemDataType.Snack) return false;
        return GetDaysElapsed(purchaseDay, currentDay) >= shelfLifeDays;
    }
    
    /// <summary>
    /// 남은 유통기한 일수 계산
    /// </summary>
    /// <param name="purchaseDay">구매한 날짜</param>
    /// <param name="currentDay">현재 날짜</param>
    /// <returns>남은 일수 (0 이하면 만료)</returns>
    public int GetRemainingDays(int purchaseDay, int currentDay)
    {
        if (dataType != StoreItemDataType.Snack) return 999;
        return shelfLifeDays - GetDaysElapsed(purchaseDay, currentDay);
    }

    // 타입 확인 헬퍼 메서드들
    public bool IsShopItem()
    {
        return dataType == StoreItemDataType.ShopItem;
    }
    
    public bool IsSnack()
    {
        return dataType == StoreItemDataType.Snack;
    }
    
    // 타입별 데이터 접근 헬퍼 메서드들
    public StoreItemData AsShopItem()
    {
        if (dataType != StoreItemDataType.ShopItem) 
        {
            Debug.LogWarning($"{itemName}은 상점 아이템이 아닙니다!");
            return null;
        }
        return this;
    }
    
    public StoreItemData AsSnack()
    {
        if (dataType != StoreItemDataType.Snack) 
        {
            Debug.LogWarning($"{itemName}은 간식 아이템이 아닙니다!");
            return null;
        }
        return this;
    }

    // Inspector에서 타입 변경 시 호출되는 메서드
    private void OnValidate()
    {
        // 타입에 따라 불필요한 필드들 초기화
        if (dataType == StoreItemDataType.ShopItem)
        {
            _effectType = SnackEffectType.Stamina;
            _itemStat = 0f;
            _shelfLifeDays = 5;
        }
        else if (dataType == StoreItemDataType.Snack)
        {
            _itemType = ItemType.Small;
            _itemCode = "";
        }
    }
}