using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public Transform contentParent; // ScrollView 안 content
    public GameObject slotPrefab;
    public IntSlotValueSO inventorySlotCount; // 업그레이드 반영된 슬롯 수
    public GameObject inventory;
    public SaveManager saveManager;

    [SerializeField] public List<InventorySlotData> slotList = new();
    public List<ItemInstanceData> savedItems = new();

    public ShopItemData currentShopItem;
    private List<ShopItemData> purchasedItems = new List<ShopItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        inventorySlotCount.OnValueChanged += UpgradeSlots;
    }

    private void OnDisable()
    {
        inventorySlotCount.OnValueChanged -= UpgradeSlots;
    }

    public void UpdateSlots(int newCount)
    {
        // 새로운 슬롯 생성
        for (int i = 0; i < newCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentParent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            slotList.Add(slotData);
        }
    }

    public void UpgradeSlots(int newCount)
    {
        // 새로운 슬롯 생성
        for (int i = 0; i < inventorySlotCount.upgradeCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentParent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            slotList.Add(slotData);
        }
    }

    public void ResetSlots()
    {
        savedItems = new List<ItemInstanceData>();
        
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        slotList.Clear();

        inventorySlotCount.SetValueWithoutNotify(20);

        UpdateSlots(inventorySlotCount.Value);
    }

    // 아이템 줍는 상황에서의 AddItem
    public bool AddItem(ItemData itemData)
    {
        // 1) 이미 같은 아이템이 들어있는 슬롯이 있는지 찾는다
        var existing = slotList.Find(s => 
            s.currentItem != null && s.currentItem.itemName == itemData.itemName);
        if (existing != null)
        {
            // 이미 있으면 수량만 +1
            existing.currentItemCount++;
            if (existing.countText != null)
                existing.countText.text = existing.currentItemCount.ToString();
            return true;
        }

        // 2) 빈 슬롯이 있으면 새 아이템으로 채운다
        foreach (var slot in slotList)
        {
            if (slot.currentItem == null)
            {
                slot.SetItem(itemData);
                slot.currentItemCount = 1;
                if (slot.countText != null)
                    slot.countText.text = "1";
                return true;
            }
        }

        // 3) 빈 슬롯이 하나도 없으면 실패
        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    // 상점에서 구매한 아이템 추가
    public bool AddShopItem(ShopItemData shopItemData)
    {
        if (shopItemData == null)
        {
            Debug.LogError("shopItemData가 null입니다!");
            return false;
        }

        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            if (slot.currentItem == null && slot.currentShopItem == null)
            {
                slot.SetShopItem(shopItemData);
                Debug.Log($"{shopItemData.itemName}이(가) 인벤토리에 추가되었습니다.");
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public List<ShopItemData> GetInventory()
    {
        return purchasedItems;
    }

    public void LoadItemToInventorySlot()
    {
        for (int i = 0; i < savedItems.Count && i < slotList.Count; i++)
        {
            ItemInstanceData sItem = savedItems[i];
            ItemData applyItem = ItemDatabase.Instance.GetItemDataById(sItem.itemID);
            if (slotList[int.Parse(sItem.slotNum.ToString())].currentItem == null && applyItem != null)
            {
                ItemData clone = Instantiate(applyItem);
                clone.value = sItem.value;
                slotList[int.Parse(sItem.slotNum.ToString())].SetItem(clone);
            }
            else
            {
                Debug.LogWarning($"이미 {sItem.slotNum} 번 슬롯에 아이템이 있습니다.");
            }
        }
    }

    public void Open() // UpGradeUI 열기
    {
        inventory.SetActive(true);
    }

    public void Exit() // UpGradeUI 닫기
    {
        inventory.SetActive(false);
    }

    // 아이템 ID로 인벤토리에서 아이템을 삭제
    public bool RemoveItemById(string itemId)
    {
        bool removed = false;
        // slotList에서 해당 아이템을 가진 슬롯을 찾아 삭제
        foreach (var slot in slotList)
        {
            if (slot.currentItem != null && slot.currentItem.itemID == itemId)
            {
                slot.ClearSlot();
                removed = true;
                break; // 한 개만 삭제 (중복 아이템이 있을 경우 첫 번째만 삭제)
            }
        }
        // savedItems에서도 해당 아이템 데이터 삭제
        int removedCount = savedItems.RemoveAll(item => item.itemID == itemId);
        return removed || removedCount > 0;
    }
}