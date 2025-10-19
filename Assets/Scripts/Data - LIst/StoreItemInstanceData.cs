using UnityEngine;

[System.Serializable]
public class StoreItemInstanceData
{
    [Header("공통 데이터")]
    public string itemID;
    public string itemName;
    public Sprite icon;
    public string description;
    public int price;
    public int slotNum;

    [Header("아이템 타입")]
    public StoreItemType itemType;

    [Header("상점 아이템 전용")]
    public ItemType shopItemType; // 물티슈 등
    public string itemCode; // 하위 호환성

    [Header("간식 전용")]
    public SnackEffectType effectType = SnackEffectType.Stamina;
    public float itemStat; // 스태미나/체력 증가량
    public int purchaseDay = -1; // 구매일
    public int shelfLifeDays = 5; // 유통기한

    public enum StoreItemType
    {
        ShopItem,
        Snack
    }

    // 통합된 StoreItemData용 생성자
    public StoreItemInstanceData(StoreItemData storeData, int slotNum, int purchaseDay = -1)
    {
        this.itemID = storeData.itemID;
        this.icon = storeData.icon;
        this.description = storeData.description;
        this.price = storeData.price;
        this.slotNum = slotNum;

        if (storeData.dataType == StoreItemDataType.ShopItem)
        {
            this.itemName = storeData.itemName;
            this.itemType = StoreItemType.ShopItem;
            this.shopItemType = storeData.itemType;
            this.itemCode = storeData.itemCode;
        }
        else if (storeData.dataType == StoreItemDataType.Snack)
        {
            this.itemName = storeData.snackName;
            this.itemType = StoreItemType.Snack;
            this.effectType = storeData.effectType;
            this.itemStat = storeData.itemStat;
            this.purchaseDay = purchaseDay;
            this.shelfLifeDays = storeData.shelfLifeDays;
        }
    }

    // 하위 호환성을 위한 생성자들
    public StoreItemInstanceData(StoreItemData shopData, int slotNum) : this(shopData, slotNum, -1)
    {
        // 상점 아이템용 (purchaseDay는 -1로 기본값)
    }

    // 간식 관련 메서드들
    public bool IsExpired(int currentDay)
    {
        if (itemType != StoreItemType.Snack || purchaseDay < 0) 
            return false;
        return (currentDay - purchaseDay) >= shelfLifeDays;
    }

    public int GetRemainingDays(int currentDay)
    {
        if (itemType != StoreItemType.Snack || purchaseDay < 0) 
            return 999;
        return shelfLifeDays - (currentDay - purchaseDay);
    }

    public int GetDaysElapsed(int currentDay)
    {
        if (itemType != StoreItemType.Snack || purchaseDay < 0) 
            return 0;
        return currentDay - purchaseDay;
    }
}
