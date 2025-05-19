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
    private int slotNum = -1;

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

    public bool AddItem(ItemData itemData)
    {
        foreach (var slot in slotList)
        {
            slotNum += 1;
            if (slot.currentItem == null) // 빈 슬롯 발견
            {
                itemData.slotNum = slotNum;
                slot.SetItem(itemData);
                savedItems.Add(new ItemInstanceData(itemData.itemID, itemData.itemName, itemData.icon, itemData.description, itemData.itemType, itemData.dirty, itemData.value, itemData.slotNum));
                slotNum = -1;
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

    public void Open() // UpGradeUI 열기
    {
        inventory.SetActive(true);
    }

    public void Exit() // UpGradeUI 닫기
    {
        inventory.SetActive(false);
    }
}