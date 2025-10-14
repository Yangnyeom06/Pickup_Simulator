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
    public asdfManager asdf;
    [SerializeField] public List<InventorySlotData> slotList = new();
    public List<ItemInstanceData> savedItems = new();
    public List<SnackInstanceData> savedSnacks = new();


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


    public bool AddItem(ItemData itemData, Item item)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].currentItem == null) // 빈 슬롯 발견
            {
                itemData.slotNum = i; // 슬롯 인덱스를 그대로 사용
                slotList[i].SetItem(itemData);
                savedItems.Add(new ItemInstanceData(item.uniqueID, itemData.itemID, itemData.itemName, itemData.icon, itemData.description, itemData.itemType, itemData.dirty, itemData.value, itemData.slotNum));
                asdf.pickUpItemCounts += 1;
                return true; // 아이템 추가 성공
            }
        }


        Debug.Log("인벤토리가 가득 찼습니다!");
        return false; // 실패
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

    
    public bool AddSnack(SnackData snackData)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].currentSnack == null) // 빈 슬롯 발견
            {
                if (slotList[i].currentSnack == snackData)
                {
                    snackData.slotNum = i;
                    slotList[i].SetSnack(snackData);
                    savedSnacks.Add(new SnackInstanceData(snackData.itemID, snackData.itemName, snackData.icon, snackData.description, snackData.itemStat, snackData.slotNum));
                }
                return true; // 아이템 추가 성공
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다!");
        return false; // 실패
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
    
    public void RemoveItemByInstance(InventorySlotData slot)
    {
        if (slotList.Contains(slot))
        {
            slotList.Remove(slot);
            Destroy(slot.gameObject);
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


}