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

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName; // 이름
    public Sprite icon; // 아이템 아이콘 (인벤토리에 들어올때 아이콘)
    public string description; // 아이템 설명
    public ItemType itemType; // 아이템 타입
    public ItemRarity itemRarity; // 아이템 희귀도
    public int dirty; // 더러움 수치 (가장 깨끗함 : 1, 가장 더러움 : 0)
    public int minValue; // 최소 가치
    public int maxValue; // 최대 가치
    public int value; // 기본 가치 (스폰될 때 최소 가치와 최대 가치 사이의 값으로 변경)
}

[CreateAssetMenu(fileName = "NewItemList", menuName = "Item/ItemList")]
public class ItemList : ScriptableObject
{
    public Item[] roadItems; // 길에서 드랍되는 아이템 리스트 
    public Item[] mountainItems; // 산에서 드랍되는 아이템 리스트 
    public Item[] elementSchoolItems; // 초등학교에서 드랍되는 아이템 리스트 
    public Item[] middleSchoolItems; // 중학교에서 드랍되는 아이템 리스트 
}