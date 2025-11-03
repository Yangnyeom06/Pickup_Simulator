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
    public List<StoreItemInstanceData> savedStoreItems = new();

    public InventoryData(List<ItemInstanceData> pickupitems, List<StoreItemInstanceData> storeitems, int slotCount)
    {
        savedPickUpItems = pickupitems;
        savedStoreItems = storeitems;
        savedSlotCounts = slotCount;
    }

    // 하위 호환성을 위한 생성자 (기존 방식 지원)
    public InventoryData(List<ItemInstanceData> pickupitems, List<StoreItemInstanceData> shopitems, List<StoreItemInstanceData> snackitems, int slotCount)
    {
        savedPickUpItems = pickupitems;
        savedStoreItems = new List<StoreItemInstanceData>();
        
        // 상점 아이템과 간식을 하나의 리스트로 통합
        if (shopitems != null) savedStoreItems.AddRange(shopitems);
        if (snackitems != null) savedStoreItems.AddRange(snackitems);
        
        savedSlotCounts = slotCount;
    }

    public static InventoryData FromData(InventoryManager inventory)
    {
        return new InventoryData(
            inventory.savedPickUpItems,
            inventory.savedStoreItems,
            inventory.inventorySlotCount.Value
        );
    }

    public void ApplyToInventory(InventoryManager inventory)
    {
        // 저장된 아이템 리스트 복사
        inventory.savedPickUpItems = new List<ItemInstanceData>(savedPickUpItems);
        inventory.savedStoreItems = new List<StoreItemInstanceData>(savedStoreItems);

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
        inventory.buyItemSlotList.Clear();

        inventory.UpdateSlots(savedSlotCounts);

        // 슬롯에 저장된 아이템 리스트 넣기
        inventory.LoadItemToInventorySlot();
    }
}