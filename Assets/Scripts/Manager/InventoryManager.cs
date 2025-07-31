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
        if (itemData == null)
        {
            Debug.LogError("AddItem: itemData가 null입니다!");
            return false;
        }

        // 1단계: 같은 아이템이 있는 슬롯을 먼저 찾아서 스택킹 시도
        for (int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            if (slot != null && slot.currentItem != null)
            {
                Debug.Log($"  슬롯 {i}: '{slot.currentItem.itemName}' (ID: {slot.currentItem.itemID}) - 개수: {slot.currentItemCount}/{slot.currentItem.maxStackSize}");
                
                if (slot.IsSameItem(itemData))
                {
                    Debug.Log($"  → 같은 아이템 발견! 스택킹 가능: {slot.CanAddMore()}");
                    
                    if (slot.CanAddMore())
                    {
                        int added = slot.AddItemCount(1);
                        if (added > 0)
                        {
                            // 스택킹 성공
                            savedItems.Add(new ItemInstanceData(item.uniqueID, itemData.itemID, itemData.itemName, itemData.icon, itemData.description, itemData.itemType, itemData.dirty, itemData.value, itemData.slotNum));
                            asdf.pickUpItemCounts += 1;
                            Debug.Log($"✅ '{itemData.itemName}' 스택킹 완료! 현재 개수: {slot.currentItemCount}");
                            return true;
                        }
                    }
                }
            }
            else if (slot != null)
            {
                Debug.Log($"  슬롯 {i}: [빈 슬롯]");
            }
        }

        // 2단계: 스택킹이 안 되면 새로운 빈 슬롯 찾기
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].currentItem == null) // 빈 슬롯 발견
            {
                itemData.slotNum = i; // 슬롯 인덱스를 그대로 사용
                slotList[i].SetItem(itemData);
                savedItems.Add(new ItemInstanceData(item.uniqueID, itemData.itemID, itemData.itemName, itemData.icon, itemData.description, itemData.itemType, itemData.dirty, itemData.value, itemData.slotNum));
                asdf.pickUpItemCounts += 1;
                Debug.Log($"'{itemData.itemName}' 새로운 슬롯에 추가됨!");
                return true; // 아이템 추가 성공
            }
        }

        // 3단계: 인벤토리가 가득 참
        Debug.Log($"인벤토리가 가득 찼습니다! '{itemData.itemName}'을(를) 추가할 수 없습니다.");
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

    public bool AddSnack(SnackData snack)
    {
        if (snack == null)
        {
            Debug.LogError("AddSnack: snack이 null입니다!");
            return false;
        }

        // slotList의 빈 슬롯에 snack을 넣는 로직을 구현해야 함
        // 예시:
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            // 완전히 빈 슬롯을 찾아야 함 (currentItem, currentShopItem, currentSnack 모두 null)
            if (slot.currentItem == null && slot.currentShopItem == null && slot.currentSnack == null)
            {
                slot.SetSnack(snack); // SetSnack은 슬롯에 스낵을 할당하는 메서드여야 함
                Debug.Log($"{snack.snackName}이(가) 인벤토리에 추가되었습니다.");
                return true;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public List<SnackData> GetPickedUpSnacks()
    {
        List<SnackData> result = new();
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            if (slot.currentSnack != null)
            {
                result.Add(slot.currentSnack);
            }
        }
        return result;
    }

    public List<SnackData> GetAllSnacks()
    {
        List<SnackData> result = new();
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            if (slot.currentSnack != null)
                result.Add(slot.currentSnack);
        }
        return result;
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

    /// <summary>
    /// 슬롯은 유지하고 아이템 내용만 제거합니다 (판매용)
    /// </summary>
    /// <param name="slot">내용을 비울 슬롯</param>
    public void ClearSlotContents(InventorySlotData slot)
    {
        if (slot == null)
        {
            Debug.LogWarning("ClearSlotContents: slot이 null입니다!");
            return;
        }

        if (slotList.Contains(slot))
        {
            // 슬롯 내용만 비우기 (슬롯 자체는 유지)
            slot.ClearSlot();
            Debug.Log($"슬롯 내용 제거 완료 - 슬롯은 유지됨");
        }
        else
        {
            Debug.LogWarning("ClearSlotContents: 해당 슬롯이 slotList에 없습니다!");
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