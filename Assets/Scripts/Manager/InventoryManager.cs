using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public Transform contentInven1Parent; // ScrollView 안 content
    public Transform contentInven2Parent; // ScrollView 안 content
    public GameObject slotPrefab;
    public IntSlotValueSO inventorySlotCount; // 업그레이드 반영된 슬롯 수
    public List<GameObject> inventory;
    [SerializeField] public List<InventorySlotData> slotList = new();
    [SerializeField] public List<InventorySlotData> buyItemSlotList = new();
    public List<ItemInstanceData> savedPickUpItems = new();
    public List<SnackItemInstanceData> savedSnackItems = new();
    public List<ShopItemInstanceData> savedShopItems = new();
    public GameObject followMouseImage;
    public TextMeshProUGUI explainText;
    public bool isInfoPanelActive = false;

    public bool isCleanMode = false;                 // 청소 모드 on/off
    public InventorySlotData wetWipeSlot = null;     // 선택된 물티슈 슬롯
    [SerializeField] public string wetWipeItemId = "100"; // 물티슈 ID (ItemData.itemID 기준)
    [SerializeField] public float cleanAmountPerUse = 0.25f; // 한 번 사용 시 dirty 증가량



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
            GameObject slot = Instantiate(slotPrefab, contentInven1Parent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            slotList.Add(slotData);
        }

        for (int i = 0; i < newCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentInven2Parent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            buyItemSlotList.Add(slotData);
        }
    }

    public void UpgradeSlots(int newCount)
    {
        // 새로운 슬롯 생성
        for (int i = 0; i < inventorySlotCount.upgradeCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentInven1Parent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            slotList.Add(slotData);
        }

        for (int i = 0; i < inventorySlotCount.upgradeCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentInven2Parent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            buyItemSlotList.Add(slotData);
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
                savedPickUpItems.Add(new ItemInstanceData(item.uniqueID, itemData.itemID, itemData.itemName, itemData.icon, itemData.description, itemData.itemType, itemData.dirty, itemData.value, itemData.slotNum, new Vector3 (0,0,0), new Quaternion (0,0,0,0)));
                DayManager.Instance.pickUpItemCounts += 1;
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
        
        // 스택킹 없이 항상 새로운 빈 슬롯에 개별적으로 저장
        for (int i = 0; i < buyItemSlotList.Count; i++)
        {
            if (buyItemSlotList[i].currentShopItem == null) // 빈 슬롯 발견
            {
                shopItemData.slotNum = i; // 슬롯 인덱스를 그대로 사용
                buyItemSlotList[i].SetShopItem(shopItemData);
                savedShopItems.Add(new ShopItemInstanceData(shopItemData.itemID, shopItemData.itemName, shopItemData.icon, shopItemData.description, shopItemData.price, shopItemData.itemType, shopItemData.itemCode, shopItemData.slotNum));
                Debug.Log($"'{shopItemData.itemName}' 새로운 슬롯에 추가됨!");
                return true; // 아이템 추가 성공
            }
            else if (buyItemSlotList[i].currentItem != null)
            {
                Debug.Log($"  슬롯 {i}: '{buyItemSlotList[i].currentItem.itemName}' (사용 중)");
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public bool AddSnack(SnackItemData snackItemData)
    {
        if (snackItemData == null)
        {
            Debug.LogError("AddSnack: snack이 null입니다!");
            return false;
        }

         // 스택킹 없이 항상 새로운 빈 슬롯에 개별적으로 저장
        for (int i = 0; i < buyItemSlotList.Count; i++)
        {
            if (buyItemSlotList[i].currentSnackItem == null) // 빈 슬롯 발견
            {
                snackItemData.slotNum = i; // 슬롯 인덱스를 그대로 사용
                buyItemSlotList[i].SetSnackItem(snackItemData);
                // 현재 날짜를 구매일로 설정 (DayManager가 있다면 사용, 없으면 0)
                int currentDay = DayManager.Instance != null ? DayManager.Instance.CalculateTotalDays() : 0;
                savedSnackItems.Add(new SnackItemInstanceData(snackItemData.itemID, snackItemData.snackName, snackItemData.icon, snackItemData.description, snackItemData.itemStat, snackItemData.slotNum, currentDay, snackItemData.shelfLifeDays));
                Debug.Log($"'{snackItemData.snackName}' 새로운 슬롯에 추가됨!");
                return true; // 아이템 추가 성공
            }
            else if (buyItemSlotList[i].currentItem != null)
            {
                Debug.Log($"  슬롯 {i}: '{buyItemSlotList[i].currentItem.itemName}' (사용 중)");
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public List<SnackItemData> GetPickedUpSnacks()
    {
        List<SnackItemData> result = new();
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            if (slot.currentSnackItem != null)
            {
                result.Add(slot.currentSnackItem);
            }
        }
        return result;
    }

    public List<SnackItemData> GetAllSnacks()
    {
        List<SnackItemData> result = new();
        foreach (var slot in slotList)
        {
            if (slot == null)
            {
                Debug.LogError("slotList에 null이 들어있습니다!");
                continue;
            }

            if (slot.currentSnackItem != null)
                result.Add(slot.currentSnackItem);
        }
        return result;
    }

    public void LoadItemToInventorySlot()
    {
        for (int i = 0; i < savedPickUpItems.Count && i < slotList.Count; i++)
        {
            ItemInstanceData sItem = savedPickUpItems[i];
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

    public void TryEnterCleanMode(InventorySlotData slot)
    {
        if (slot == null || slot.currentItem == null)
        {
            Debug.Log("[청소모드] 유효하지 않은 슬롯입니다.");
            return;
        }

        if (slot.currentItem.itemID != wetWipeItemId)
        {
            Debug.Log("[청소모드] 이 슬롯은 물티슈가 아닙니다. 물티슈(ID=100) 슬롯을 클릭하세요.");
            return;
        }

        // 토글 동작: 이미 청소 모드이고 같은 슬롯이면 취소
        if (isCleanMode && wetWipeSlot == slot)
        {
            isCleanMode = false;
            wetWipeSlot = null;
            Debug.Log("[청소모드] 취소되었습니다. 일반 모드로 돌아갑니다.");
            return;
        }

        // 청소 모드 진입
        isCleanMode = true;
        wetWipeSlot = slot;
        Debug.Log("[청소모드] 활성화! 깨끗하게 만들 아이템 슬롯을 클릭하세요. (다시 물티슈 슬롯을 클릭하면 취소)");
    }

    public void ApplyWetWipeTo(InventorySlotData targetSlot)
    {
        if (!isCleanMode || wetWipeSlot == null)
        {
            Debug.LogWarning("[청소모드] 활성화되지 않았습니다. 먼저 물티슈 슬롯을 클릭하세요.");
            return;
        }
        if (targetSlot == null || targetSlot.currentItem == null)
        {
            Debug.LogWarning("[청소모드] 대상 슬롯이 비어있습니다.");
            return;
        }
        if (targetSlot == wetWipeSlot)
        {
            Debug.LogWarning("[청소모드] 물티슈 슬롯 자체에는 사용할 수 없습니다. 다른 아이템을 클릭하세요.");
            return;
        }

        var item = targetSlot.currentItem;
        float before = item.dirty;
        float after = Mathf.Clamp01(before + cleanAmountPerUse);
        item.dirty = after;

        Debug.Log($"[청소모드] '{item.itemName}'의 dirty를 {before:0.00} → {after:0.00} 로 증가(더 깨끗함).");

        // 물티슈 소모: 현재 구조상 개별 슬롯 1개씩 담기므로 바로 Clear
        wetWipeSlot.ClearSlot();
        Debug.Log("[청소모드] 물티슈 1개를 사용했습니다. 슬롯이 비워졌습니다.");

        // 청소 모드 종료
        isCleanMode = false;
        wetWipeSlot = null;
        Debug.Log("[청소모드] 종료되었습니다.");
    }

    public void Open()
    {
        inventory[0].SetActive(true);
    }

    public void Exit()
    {
        inventory[0].SetActive(false);
        inventory[1].SetActive(false);
    }

    public void ResetSlots()
    {
        savedPickUpItems = new List<ItemInstanceData>();
        savedSnackItems = new List<SnackItemInstanceData>();
        savedShopItems = new List<ShopItemInstanceData>();

        foreach (Transform child in contentInven1Parent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in contentInven2Parent)
        {
            Destroy(child.gameObject);
        }


        slotList.Clear();

        inventorySlotCount.SetValueWithoutNotify(20);

        UpdateSlots(inventorySlotCount.Value);
    }
}