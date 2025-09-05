using UnityEngine;
using System.Collections.Generic;

public class ShopItemDatabase : MonoBehaviour
{
    public static ShopItemDatabase Instance { get; private set; }

    [SerializeField] private List<ShopItemData> allItems = new();

    private Dictionary<string, ShopItemData> shopItemDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var shopItem in allItems)
        {
            if (!shopItemDict.ContainsKey(shopItem.itemID))
                shopItemDict.Add(shopItem.itemID, shopItem);
            else
                Debug.LogWarning($"중복된 shopItemID: {shopItem.itemID}");
        }
    }

    public ShopItemData GetShopItemDataById(string id)
    {
        if (shopItemDict.TryGetValue(id, out ShopItemData shopItem))
            return shopItem;
        return null;
    }

    // 하위 호환성을 위한 메서드
    public ShopItemData GetShopItemDataByCode(string itemCode)
    {
        foreach (var shopItem in allItems)
        {
            if (shopItem.itemCode == itemCode)
                return shopItem;
        }
        return null;
    }
}
