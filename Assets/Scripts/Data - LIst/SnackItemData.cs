using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[CreateAssetMenu(fileName = "NewSnackItem", menuName = "SnackItem/SnackItemData")]
public class SnackItemData : ScriptableObject
{
    public string itemID;
    public string snackName; // 간식 이름
    public Sprite icon; // 간식 아이콘 (인벤토리에 들어올때 아이콘)
    public int price;                   // 가격
    public string description; // 어떤 능력을 갖고 있는지 설명
    public float itemStat; // 간식을 먹으면 스테미나 증가(10~30)
    public int slotNum;
}