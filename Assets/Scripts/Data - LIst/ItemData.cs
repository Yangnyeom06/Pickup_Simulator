using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum ItemType {
    Large, // 큰 아이템
    Small, // 작은 아이템
    Consumable // 소모성 아이템
}

public enum ItemRarity {
    Common,
    Rare,
    Unique
}

public enum DirtyState {
    VeryCleaning = 0,  // 매우깨끗한 상태 (0.8~1.0) - 가격 감소 없음
    Clean = 1,         // 깨끗한 상태 (0.6~0.8) - 5G 감소
    Normal = 2,        // 보통 상태 (0.4~0.6) - 10G 감소
    Dirty = 3,         // 더러운 상태 (0.2~0.4) - 15G 감소
    VeryDirty = 4      // 매우 더러운 상태 (0.0~0.2) - 20G 감소
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName; // 이름
    public Sprite icon; // 아이템 아이콘 (인벤토리에 들어올때 아이콘)
    public int price; // 아이템 가격
    public string description; // 아이템 설명
    public ItemType itemType; // 아이템 타입
    public ItemRarity itemRarity; // 아이템 희귀도
    public float dirty; // 더러움 수치 (가장 깨끗함 : 1, 가장 더러움 : 0)
    public int minValue; // 최소 가치
    public int maxValue; // 최대 가치
    public int value; // 기본 가치 (스폰될 때 최소 가치와 최대 가치 사이의 값으로 변경)
    public int maxStackSize = 99; // 최대 스택 크기 (기본값: 99개)
    public int slotNum;
    
    /// <summary>
    /// 더러움 수치를 기반으로 현재 더러움 상태를 반환
    /// </summary>
    /// <returns>더러움 상태</returns>
    public DirtyState GetDirtyState()
    {
        if (dirty >= 0.8f) return DirtyState.VeryCleaning;
        else if (dirty >= 0.6f) return DirtyState.Clean;
        else if (dirty >= 0.4f) return DirtyState.Normal;
        else if (dirty >= 0.2f) return DirtyState.Dirty;
        else return DirtyState.VeryDirty;
    }
    
    /// <summary>
    /// 더러움 상태에 따른 가격 감소량을 반환
    /// </summary>
    /// <returns>감소할 가격</returns>
    public int GetDirtyPenalty()
    {
        switch (GetDirtyState())
        {
            case DirtyState.VeryCleaning: return 0;   // 매우깨끗한 상태 - 감소 없음
            case DirtyState.Clean: return 5;          // 깨끗한 상태 - 5G 감소
            case DirtyState.Normal: return 10;        // 보통 상태 - 10G 감소
            case DirtyState.Dirty: return 15;         // 더러운 상태 - 15G 감소
            case DirtyState.VeryDirty: return 20;     // 매우 더러운 상태 - 20G 감소
            default: return 0;
        }
    }
    
    /// <summary>
    /// 더러움을 고려한 실제 판매 가격을 계산
    /// </summary>
    /// <returns>더러움 페널티를 적용한 판매 가격</returns>
    public int GetAdjustedSalePrice()
    {
        int penalty = GetDirtyPenalty();
        int adjustedPrice = Mathf.Max(price - penalty, 1); // price를 기준으로 페널티 적용, 최소 1G는 유지
        return adjustedPrice;
    }
    
    /// <summary>
    /// 더러움 상태를 한국어 문자열로 반환
    /// </summary>
    /// <returns>더러움 상태 문자열</returns>
    public string GetDirtyStateString()
    {
        switch (GetDirtyState())
        {
            case DirtyState.VeryCleaning: return "매우 깨끗함";
            case DirtyState.Clean: return "깨끗함";
            case DirtyState.Normal: return "보통";
            case DirtyState.Dirty: return "더러움";
            case DirtyState.VeryDirty: return "매우 더러움";
            default: return "알 수 없음";
        }
    }
}