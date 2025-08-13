using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    public string uniqueID;
    public ItemData itemData; // 아이템 데이터 참조
    InventoryManager inventoryManager;
/*
    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null)
        {
            if (inventoryManager.AddItem(itemData)) // 성공한 경우만 삭제
            {
                DeleteItem();
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
*/
    public void SetItemData(ItemData data)
    {
        itemData = data;
    }

    public void AssignUniqueId()
    {
        uniqueID = Guid.NewGuid().ToString();
    }
}