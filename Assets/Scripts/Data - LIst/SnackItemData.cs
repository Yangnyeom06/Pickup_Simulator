using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum SnackEffectType
{
    Stamina = 0,    // 스태미나 증가
    Health = 1,     // 체력 증가
    Both = 2        // 체력과 스태미나 동시 증가
}

[CreateAssetMenu(fileName = "NewSnackItem", menuName = "SnackItem/SnackItemData")]
public class SnackItemData : ScriptableObject
{
    public string itemID;
    public string snackName; // 간식 이름
    public Sprite icon; // 간식 아이콘 (인벤토리에 들어올때 아이콘)
    public int price;                   // 가격
    public string description; // 어떤 능력을 갖고 있는지 설명
    
    [Header("간식 효과")]
    public SnackEffectType effectType = SnackEffectType.Stamina; // 효과 타입 (기본: 스태미나)
    public float itemStat; // 간식을 먹으면 스테미나 또는 체력 증가(10~30)
    public int slotNum;
    
    /// <summary>
    /// 구매일로부터 며칠이 지났는지 계산
    /// </summary>
    /// <param name="purchaseDay">구매한 날짜 (총 경과 일수)</param>
    /// <param name="currentDay">현재 날짜 (총 경과 일수)</param>
    /// <returns>경과 일수</returns>
    public int GetDaysElapsed(int purchaseDay, int currentDay)
    {
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
        return shelfLifeDays - GetDaysElapsed(purchaseDay, currentDay);
    }
}