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
    public List<ItemInstanceData> savedPickUpItems = new();
    public List<ShopItemInstanceData> savedShopItems = new();
    public List<SnackItemInstanceData> savedSnackItems = new();

    public InventoryData(List<ItemInstanceData> pickupitems, List<ShopItemInstanceData> shopitems, List<SnackItemInstanceData> snackitems, int slotCount)
    {
        savedPickUpItems = pickupitems;
        savedShopItems = shopitems;
        savedSnackItems = snackitems;
        savedSlotCounts = slotCount;
    }

    public static InventoryData FromData(InventoryManager inventory)
    {
        return new InventoryData(
            inventory.savedPickUpItems,
            inventory.savedShopItems,
            inventory.savedSnackItems,
            inventory.inventorySlotCount.Value
        );
    }

    public void ApplyToInventory(InventoryManager inventory)
    {
        // 저장된 아이템 리스트 복사
        inventory.savedPickUpItems = new List<ItemInstanceData>(savedPickUpItems);
        inventory.savedShopItems = new List<ShopItemInstanceData>(savedShopItems);
        inventory.savedSnackItems = new List<SnackItemInstanceData>(savedSnackItems);

        inventory.inventorySlotCount.SetValueWithoutNotify(savedSlotCounts);
        
        foreach (Transform child in inventory.contentInven1Parent)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        foreach (Transform child in inventory.contentInven2Parent)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        inventory.slotList.Clear();

        inventory.UpdateSlots(savedSlotCounts);

        // 슬롯에 저장된 아이템 리스트 넣기
        inventory.LoadItemToInventorySlot();
    }
}