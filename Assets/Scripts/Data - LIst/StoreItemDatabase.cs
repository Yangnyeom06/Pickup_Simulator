using UnityEngine;
using System.Collections.Generic;

public class StoreItemDatabase : MonoBehaviour
{
    public static StoreItemDatabase Instance { get; private set; }

    [Header("모든 상점 아이템들 (통합)")]
    [SerializeField] private List<StoreItemData> allStoreItems = new();

    private Dictionary<string, StoreItemData> storeItemDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // StoreItem 딕셔너리 초기화 (통합)
        foreach (var storeItem in allStoreItems)
        {
            if (!storeItemDict.ContainsKey(storeItem.itemID))
                storeItemDict.Add(storeItem.itemID, storeItem);
            else
                Debug.LogWarning($"중복된 storeItemID: {storeItem.itemID}");
        }
    }

    // 통합 StoreItem 관련 메서드들
    public StoreItemData GetStoreItemDataById(string id)
    {
        if (storeItemDict.TryGetValue(id, out StoreItemData storeItem))
            return storeItem;
        return null;
    }

    public StoreItemData GetStoreItemDataByCode(string itemCode)
    {
        foreach (var storeItem in allStoreItems)
        {
            if (storeItem.dataType == StoreItemDataType.ShopItem && storeItem.itemCode == itemCode)
                return storeItem;
        }
        return null;
    }

    // 하위 호환성을 위한 메서드들
    public StoreItemData GetShopItemDataById(string id)
    {
        var item = GetStoreItemDataById(id);
        return (item != null && item.dataType == StoreItemDataType.ShopItem) ? item : null;
    }

    public StoreItemData GetShopItemDataByCode(string itemCode)
    {
        return GetStoreItemDataByCode(itemCode);
    }

    public StoreItemData GetSnackItemDataById(string id)
    {
        var item = GetStoreItemDataById(id);
        return (item != null && item.dataType == StoreItemDataType.Snack) ? item : null;
    }

    // 통합 메서드들
    public List<StoreItemData> GetAllStoreItems()
    {
        return new List<StoreItemData>(allStoreItems);
    }

    public List<StoreItemData> GetAllShopItems()
    {
        return allStoreItems.FindAll(item => item.dataType == StoreItemDataType.ShopItem);
    }

    public List<StoreItemData> GetAllSnackItems()
    {
        return allStoreItems.FindAll(item => item.dataType == StoreItemDataType.Snack);
    }

    // StoreItem과 연동을 위한 메서드
    public bool IsShopItem(string itemID)
    {
        var item = GetStoreItemDataById(itemID);
        return item != null && item.dataType == StoreItemDataType.ShopItem;
    }

    public bool IsSnackItem(string itemID)
    {
        var item = GetStoreItemDataById(itemID);
        return item != null && item.dataType == StoreItemDataType.Snack;
    }

    public bool ContainsItem(string itemID)
    {
        return storeItemDict.ContainsKey(itemID);
    }
}
