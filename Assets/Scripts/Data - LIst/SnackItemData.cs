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
    
    [Header("유통기한 설정")]
    public int shelfLifeDays = 5; // 유통기한 (일수) - 기본값 5일
}