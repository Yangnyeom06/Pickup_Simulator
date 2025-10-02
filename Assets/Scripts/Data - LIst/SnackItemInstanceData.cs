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
    public int purchaseDay = -1; // 기본값: 구매일 미설정
    public int shelfLifeDays = 5; // 기본 유통기한 5일


    // 구매일과 유통기한을 포함한 생성자
    public SnackInstanceData(string itemID, string itemName, Sprite icon, string description, float itemStat, int slotNum, int purchaseDay, int shelfLifeDays)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.itemStat = itemStat;
        this.slotNum = slotNum;
        this.purchaseDay = purchaseDay;
        this.shelfLifeDays = shelfLifeDays;
    }
    
    /// <summary>
    /// 간식이 만료되었는지 확인
    /// </summary>
    /// <param name="currentDay">현재 날짜</param>
    /// <returns>만료되었으면 true</returns>
    public bool IsExpired(int currentDay)
    {
        if (purchaseDay < 0) return false; // 구매일이 설정되지 않았으면 만료되지 않음
        return (currentDay - purchaseDay) >= shelfLifeDays;
    }
    
    /// <summary>
    /// 남은 유통기한 일수 계산
    /// </summary>
    /// <param name="currentDay">현재 날짜</param>
    /// <returns>남은 일수 (0 이하면 만료)</returns>
    public int GetRemainingDays(int currentDay)
    {
        if (purchaseDay < 0) return 999; // 구매일이 설정되지 않았으면 무한
        return shelfLifeDays - (currentDay - purchaseDay);
    }
}

