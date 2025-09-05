using UnityEngine;

[System.Serializable]
public class ShopItemInstanceData
{
    public string itemID;
    public string itemName; // 상점 아이템 이름
    public Sprite icon; // 상점 아이템 아이콘 (인벤토리에 들어올때 아이콘)
    public string description; // 아이템 설명
    public int price; // 가격
    public ItemType itemType; // 아이템 타입
    public string itemCode; // 하위 호환성을 위한 필드
    public int slotNum; // 인벤토리 슬롯 번호

    public ShopItemInstanceData(string itemID, string itemName, Sprite icon, string description, int price, ItemType itemType, string itemCode, int slotNum)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.price = price;
        this.itemType = itemType;
        this.itemCode = itemCode;
        this.slotNum = slotNum;
    }
}
