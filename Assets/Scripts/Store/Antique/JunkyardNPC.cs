using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class JunkyardNPC : MonoBehaviour, ISaleSystem
{
    public Camera mainCamera;
    public float rayDistance = 100f;

    public PlayerData playerData;

    // [Header("Inspector 에서 드래그해서 지정할 클릭 대상들")]
    // public List<Transform> clickableTargets;

    [Header("UI References")]
    public GameObject sellUI;                   // Sell 모드 전체 패널
    public GameObject slotPrefab;               // 슬롯 프리팹 (InventorySlotData 컴포넌트 포함)
    public Transform  slotParent;               // 슬롯이 붙을 부모 (Layout Group 등)
    public Button     sellConfirmButton;        // 최종 판매 확정 버튼
    public GameObject quantityDialogPrefab;     // QuantityDialog 프리팹
    public Transform  quantityDialogParent;     // 다이얼로그를 붙일 부모

    [Header("Managers")]
    public MoneyManager     moneyManager;
    public InventoryManager inventoryManager;

    // 현재 선택된 슬롯·수량
    private InventorySlotData selectedSlot;
    public int selectedQuantity { get; set; }
    private QuantityDialog    quantityDialog;

    [SerializeField] private TMP_Text playerMoneyText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    ShowSellUI();
                }
            }
        }
    }

    public void ShowSellUI()
    {
        sellUI.SetActive(true);
        sellConfirmButton.interactable = false;
        ResetSaleState();

         if (quantityDialog != null)
             quantityDialog.gameObject.SetActive(false);

         sellConfirmButton.interactable = false;
         RefreshSellSlots();
    }


    /// (2) 인벤토리 데이터 → SellUI 슬롯으로 복제
    public void RefreshSellSlots()
    {
        // 참조 누락 방어
        if (slotParent == null || slotPrefab == null || inventoryManager == null)
        {
            Debug.LogError("[JunkyardSystem] slotParent/slotPrefab/inventoryManager 설정이 필요합니다!");
            return;
        }

        // 기존 슬롯 전부 삭제
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
            
        // InventoryManager.slotList 순회
        foreach (var data in inventoryManager.slotList)
        {
            var item = data.currentItem;
            if (item == null || data.currentItemCount <= 0)
                continue;

            if (!CanSell(item))
                continue;

            var go = Instantiate(slotPrefab, slotParent);
            var ui = go.GetComponent<InventorySlotData>();
            ui.SetupSlot(item, data.currentItemCount, this);
        }

        // 소지금 표시
        int playerMoney = playerData.money;
        playerMoneyText.text = $"Money: {playerMoney} G";
    }

    // (3) 슬롯 클릭 → 다이얼로그 띄우기 + 판매 확정 버튼 활성
    public void OnSlotClicked(InventorySlotData slot)
    {
        if (slot == null || slot.currentItem == null)
            return;

        // 대형 아이템 여부 재확인
        if (slot.currentItem.itemType != ItemType.Large)
        {
            Debug.Log("[JunkyardSystem] 대형 아이템만 판매할 수 있습니다.");
            return;
        }

        selectedSlot     = slot;
        selectedQuantity = 1;

        if (quantityDialog != null)
            Destroy(quantityDialog.gameObject);

        var dlgGO = Instantiate(quantityDialogPrefab, quantityDialogParent);
        quantityDialog = dlgGO.GetComponent<QuantityDialog>();

        quantityDialog.Setup(
            this,
            slot.currentItem,
            slot.currentItemCount
        );

        sellConfirmButton.interactable = true;

    }

    public bool CanSell(ItemData item)
    {
        return item != null && item.itemType == ItemType.Large && item.value > 0;
    }

    public void IncreaseQuantity(int maxQty)
    {
        selectedQuantity = Mathf.Min(selectedQuantity + 1, maxQty);
        if (quantityDialog != null)
            quantityDialog.UpdateQuantity(selectedQuantity);
    }

    public void DecreaseQuantity()
    {
        selectedQuantity = Mathf.Max(selectedQuantity - 1, 1);
        if (quantityDialog != null)
            quantityDialog.UpdateQuantity(selectedQuantity);
    }

    // 판매 버튼 클릭 시 호출
    public void ConfirmSell()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null)
        return;

        // 1) 판매 정보 미리 저장
        var itemData  = selectedSlot.currentItem;
        var itemName = itemData.itemName;
        var itemID    = itemData.itemID;
        int sellCount = selectedQuantity;
        int gain      = itemData.value * sellCount;

        // 2) 돈 입금
        moneyManager.AddMoney(gain);

        // 3) 슬롯에서 수량 차감
        selectedSlot.RemoveItem(sellCount);

        // 4) 남은 수량이 0이면, 저장된 itemID로 슬롯 자체 삭제
        if (selectedSlot.currentItemCount <= 0)
        {
            inventoryManager.RemoveItemById(itemID);
            Destroy(selectedSlot.gameObject);
        }

        // 5) UI 갱신
        RefreshSellSlots();
        ResetSaleState(); 

        // // 6) 다이얼로그 & 상태 초기화
        // if (quantityDialog != null) Destroy(quantityDialog.gameObject);
        // quantityDialog    = null;
        // selectedSlot      = null;
        // sellConfirmButton.interactable = false;

        Debug.Log($"판매 완료: +{gain}G");
        playerMoneyText.text = $"Money: {playerData.money} G";
    }

    private void ResetSaleState()
    {
        if (quantityDialog != null)
        {
            Destroy(quantityDialog.gameObject);
            quantityDialog = null;
        }

        selectedSlot = null;
        selectedQuantity = 0;
        sellConfirmButton.interactable = false;
    }

    public void CancelSell() 
    {
        sellUI.SetActive(false);
        ResetSaleState();
    }
}
