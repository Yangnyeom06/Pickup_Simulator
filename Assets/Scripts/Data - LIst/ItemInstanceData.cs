using UnityEngine;

[System.Serializable]
public class ItemInstanceData
{
    public string uniqueID; // 고유 ID
    public string itemID;       // SO의 식별자 (예: 이름 또는 GUID).
    public string itemName; // 이름
    public Sprite icon; // 아이템 아이콘 (인벤토리에 들어올때 아이콘)
    public string description; // 아이템 설명
    public ItemType itemType; // 아이템 타입
    public ItemRarity itemRarity; // 아이템 희귀도
    public float dirty; // 더러움 수치 (가장 깨끗함 : 1, 가장 더러움 : 0)
    public int value;
    public int slotNum;

    public Vector3 position;
    public Quaternion rotation;


    public ItemInstanceData(string uniqueID, string itemID, string itemName, Sprite icon, string description, ItemType itemType, float dirty, int value, int slotNum, Vector3 position, Quaternion rotation)
    {
        this.uniqueID = uniqueID;
        this.itemID = itemID;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.itemType = itemType;
        this.dirty = dirty;
        this.value = value;
        this.slotNum = slotNum;
        this.position = position;
        this.rotation = rotation;
    }
}
