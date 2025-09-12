using UnityEngine;

[System.Serializable]
public class SnackItemInstanceData
{
    public string itemID;
    public string itemName; // 간식 이름
    public Sprite icon; // 간식 아이콘 (인벤토리에 들어올때 아이콘)
    public string description; // 어떤 능력을 갖고 있는지 설명
    public float itemStat; // 간식을 먹으면 스테미나 증가(10~30)
    public int slotNum;


    public SnackItemInstanceData(string itemID, string itemName, Sprite icon, string description, float itemStat, int slotNum)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.itemStat = itemStat;
        this.slotNum = slotNum;
    }
}
