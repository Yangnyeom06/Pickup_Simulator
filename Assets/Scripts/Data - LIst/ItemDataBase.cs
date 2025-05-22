using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> allItems = new();

    private Dictionary<string, ItemData> itemDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var item in allItems)
        {
            if (!itemDict.ContainsKey(item.itemID))
                itemDict.Add(item.itemID, item);
            else
                Debug.LogWarning($"중복된 itemID: {item.itemID}");
        }
    }

    public ItemData GetItemDataById(string id)
    {
        if (itemDict.TryGetValue(id, out ItemData item))
            return item;
        return null;
    }
}
