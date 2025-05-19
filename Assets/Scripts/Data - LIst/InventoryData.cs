using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
[Serializable]
public class InventoryData
{
    public int savedSlotCounts;
    public List<ItemInstanceData> savedItems = new();

    public InventoryData(List<ItemInstanceData> items, int slotCount)
    {
        savedItems = items;
        savedSlotCounts = slotCount;
    }

    public static InventoryData FromData(InventoryManager inventory)
    {
        return new InventoryData(
            inventory.savedItems,
            inventory.inventorySlotCount.Value
        );
    }

    public void ApplyToInventory(InventoryManager inventory)
    {
        // 저장된 아이템 리스트 복사
        inventory.savedItems = new List<ItemInstanceData>(savedItems);

        inventory.inventorySlotCount.SetValueWithoutNotify(savedSlotCounts);
        
        foreach (Transform child in inventory.contentParent)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        inventory.slotList.Clear();

        inventory.UpdateSlots(savedSlotCounts);

        // 슬롯에 저장된 아이템 리스트 넣기
        inventory.LoadItemToInventorySlot();
    }
}