using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaleSystem : MonoBehaviour
{
    [Header("UI References")]
    public Button     sellButton;               // NPC 클릭 후 Sell 버튼
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
    [HideInInspector] public  int                  selectedQuantity;
    private QuantityDialog    quantityDialog;

    private void Awake()
    {
        // 1) Sell 버튼 세팅
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellButtonClicked);

        // 2) 판매 확정 버튼 초기 비활성
        sellConfirmButton.onClick.RemoveAllListeners();
        sellConfirmButton.onClick.AddListener(ConfirmSell);
        sellConfirmButton.interactable = false;

        // 3) 시작 시 SellUI 닫기
        sellUI.SetActive(false);
    }

    /// <summary>
    /// (1) Sell 버튼 클릭
    /// </summary>
    public void OnSellButtonClicked()
    {
        // SellUI 열고, Sell 버튼 숨기고, 확정 비활성
        sellUI.SetActive(true);
        sellButton.gameObject.SetActive(false);
        sellConfirmButton.interactable = false;

        // 슬롯 리스트 갱신
        RefreshSellSlots();
    }

    /// <summary>
    /// (2) 인벤토리 데이터 → SellUI 슬롯으로 복제
    /// </summary>
    public void RefreshSellSlots()
    {
        // 참조 누락 방어
        if (slotParent == null || slotPrefab == null || inventoryManager == null)
        {
            Debug.LogError("[SaleSystem] slotParent/slotPrefab/inventoryManager 설정이 필요합니다!");
            return;
        }

        // 기존 슬롯 전부 삭제
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        // InventoryManager.slotList 순회
        foreach (var data in inventoryManager.slotList)
        {
            if (data.currentItem == null || data.currentItemCount <= 0)
                continue;

            // 슬롯 복제
            var go = Instantiate(slotPrefab, slotParent);
            var ui = go.GetComponent<InventorySlotData>();
            ui.SetupSlot(
                data.currentItem,
                data.currentItemCount,
                this
            );
        }
    }

    /// <summary>
    /// (3) 슬롯 클릭 → 다이얼로그 띄우기 + 판매 확정 버튼 활성
    /// </summary>
    public void OnSlotClicked(InventorySlotData slot)
    {
        if (slot == null || slot.currentItem == null) return;

        selectedSlot     = slot;
        selectedQuantity = 1;

        // 기존 다이얼로그 제거
        if (quantityDialog != null)
            Destroy(quantityDialog.gameObject);

        // 다이얼로그 생성
        var dlgGO = Instantiate(quantityDialogPrefab, quantityDialogParent);
        quantityDialog   = dlgGO.GetComponent<QuantityDialog>();

        // ★ 여기서 반드시 ItemData 와 maxQty(=slot.currentItemCount) 두 개를 넘겨줍니다.
        quantityDialog.Setup(
            this,                     // SaleSystem 인스턴스
            slot.currentItem,         // 판매할 아이템 정보
            slot.currentItemCount     // 최대 선택 가능한 수량
        );

        // 판매 확정 버튼 활성화 등…
        sellConfirmButton.interactable = true;
    }

    /// <summary>
    /// (4) 수량 + 버튼
    /// </summary>
    public void IncreaseQuantity(int maxQuantity)
    {
        selectedQuantity = Mathf.Min(selectedQuantity + 1, maxQuantity);
        if (quantityDialog != null)
            quantityDialog.UpdateQuantity(selectedQuantity);
    }

    /// <summary>
    /// (4) 수량 – 버튼
    /// </summary>
    public void DecreaseQuantity()
    {
        selectedQuantity = Mathf.Max(selectedQuantity - 1, 1);
        if (quantityDialog != null)
            quantityDialog.UpdateQuantity(selectedQuantity);
    }

    /// <summary>
    /// (5) 판매 확정 버튼 클릭
    /// </summary>
    public void ConfirmSell()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null)
        return;

        // 1) 판매 정보 미리 저장
        var itemData  = selectedSlot.currentItem;
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
        }

        // 5) UI 갱신
        RefreshSellSlots();

        // 6) 다이얼로그 & 상태 초기화
        if (quantityDialog != null) Destroy(quantityDialog.gameObject);
        quantityDialog    = null;
        selectedSlot      = null;
        selectedQuantity  = 1;
        sellConfirmButton.interactable = false;

        // 7) 판매 화면 닫기
        sellUI.SetActive(false);
        sellButton.gameObject.SetActive(false);

        Debug.Log($"판매 완료: +{gain}G");
    }

    public void CancelSell() 
    {
        sellUI.SetActive(false);
    }
}