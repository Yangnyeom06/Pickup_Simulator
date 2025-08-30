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
    public List<ShopItemInstanceData> savedShopItems = new();
    private bool isAddingSnack = false; // 스낵 추가 중복 방지 플래그


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // 게임 시작 시 슬롯이 비어있다면 초기화
        if (slotList.Count == 0)
        {
            Debug.Log("슬롯이 비어있어서 초기화합니다.");
            InitializeSlots();
        }
    }

    private void OnEnable()
    {
        inventorySlotCount.OnValueChanged += UpgradeSlots;
    }

    private void OnDisable()
    {
        inventorySlotCount.OnValueChanged -= UpgradeSlots;
    }

    // 초기 슬롯 생성
    private void InitializeSlots()
    {
        // 기본 20개 슬롯 생성
        inventorySlotCount.SetValueWithoutNotify(20);
        UpdateSlots(inventorySlotCount.Value);
        Debug.Log($"인벤토리 슬롯 {inventorySlotCount.Value}개 생성 완료!");
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

        Debug.Log($"아이템 추가 시도: '{itemData.itemName}' - 개별 슬롯에 저장");

        // 스택킹 없이 항상 새로운 빈 슬롯에 개별적으로 저장
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
            else if (slotList[i].currentItem != null)
            {
                Debug.Log($"  슬롯 {i}: '{slotList[i].currentItem.itemName}' (사용 중)");
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
        Debug.Log($"⚠️ AddSnack 중복 호출 감지: {snack?.snackName ?? "null"}");
        Debug.Log($"호출 스택: {System.Environment.StackTrace}");
        
        // 중복 호출 방지
        if (isAddingSnack)
        {
            Debug.LogWarning("⚠️ 중복 호출 차단됨!");
            return false;
        }
        
        isAddingSnack = true;
        
        if (snack == null)
        {
            isAddingSnack = false;
            return false;
        }

        // slotList의 빈 슬롯에 snack을 넣는 로직을 구현해야 함
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                continue;
            }

            // 완전히 빈 슬롯을 찾아야 함 (currentItem, currentShopItem, currentSnack 모두 null)
            if (slot.currentItem == null && slot.currentShopItem == null && slot.currentSnack == null)
            {
                slot.SetSnack(snack);
                isAddingSnack = false;
                return true;
            }
        }
        isAddingSnack = false;
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
        savedSnacks = new List<SnackInstanceData>();
        savedShopItems = new List<ShopItemInstanceData>();

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        slotList.Clear();

        inventorySlotCount.SetValueWithoutNotify(20);

        UpdateSlots(inventorySlotCount.Value);
    }


}