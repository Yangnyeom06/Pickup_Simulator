using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaleSystem : MonoBehaviour, ISaleSystem
{
    public Camera mainCamera;
    public float rayDistance = 100f;

    public PlayerData playerData;
    
    [Header("UI References")]
    public Button     sellButton;               // NPC 클릭 후 Sell 버튼
    public GameObject sellUI;                   // Sell 모드 전체 패널
    public GameObject slotPrefab;               // 슬롯 프리팹 (InventorySlotData 컴포넌트 포함)
    public Transform  slotParent;               // 슬롯이 붙을 부모 (Layout Group 등)
    public Button     sellConfirmButton;        // 최종 판매 확정 버튼
    // public GameObject quantityDialogPrefab;     // QuantityDialog 프리팹
    // public Transform  quantityDialogParent;     // 다이얼로그를 붙일 부모
    public TMP_Text playerMoneyText;

    [Header("Managers")]
    public MoneyManager     moneyManager;
    public InventoryManager inventoryManager;

    // 현재 선택된 슬롯·수량
    private InventorySlotData selectedSlot;
    // public int selectedQuantity {get; set;} = 0;
    // private QuantityDialog    quantityDialog;

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

    // 아이템 습득 시 호출되는 메서드
    public void OnItemPickedUp()
    {
        // 판매 UI가 활성화된 경우에만 슬롯 갱신
        if (sellUI != null && sellUI.activeSelf)
        {
            RefreshSellSlots();
        }
    }

    // Sell 버튼 클릭
    public void OnSellButtonClicked()
    {
        // SellUI 열고, Sell 버튼 숨기고, 확정 비활성
        sellUI.SetActive(true);
        sellButton.gameObject.SetActive(false);
        // quantityDialogPrefab.SetActive(false);
        ResetSaleState();

        // 슬롯 리스트 갱신
        RefreshSellSlots();
    }

    // (2) 인벤토리 데이터 → SellUI 슬롯으로 복제
    public void RefreshSellSlots()
    {
        // 기존 슬롯 전부 삭제
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
            
        // InventoryManager.slotList 순회
        foreach (var data in inventoryManager.slotList)
        {
            var item = data.currentItem;

            if (item == null || data.currentItemCount <= 0) continue;
            if (!CanSell(item)) continue;


            for (int i = 0; i < data.currentItemCount; i++)
            {
                var go = Instantiate(slotPrefab, slotParent);
                var ui = go.GetComponent<InventorySlotData>();
    
                ui.SetupSlot(item, 1, this); // 슬롯 하나에 아이템 1개
                ui.originalInventorySlot = data;
            }

        }
    }

    // (3) 슬롯 클릭 → 판매 확정 버튼 활성
    public void OnSlotClicked(InventorySlotData slot)
    {
        if (slot == null || slot.currentItem == null) return;

        selectedSlot     = slot;
        // selectedQuantity = 1;

        // // 기존 다이얼로그 제거
        // if (quantityDialog != null)
        // {
        //     Destroy(quantityDialog.gameObject);
        //     quantityDialog = null;
        // }

        // 다이얼로그 생성
        // quantityDialogPrefab.SetActive(true);
        // var dlgGO = Instantiate(quantityDialogPrefab, quantityDialogParent, false);
        // quantityDialog   = dlgGO.GetComponent<QuantityDialog>();
        // quantityDialog.Setup(
        //     this,                     // SaleSystem 인스턴스
        //     slot.currentItem,         // 판매할 아이템 정보
        //     slot.currentItemCount     // 최대 선택 가능한 수량
        // );

        // 판매 확정 버튼 활성화 등…
        sellConfirmButton.interactable = true;
    }

    public bool CanSell(ItemData item)
    {
        return true;
    }

    // (4) 수량 + 버튼>
    // public void IncreaseQuantity(int maxQuantity)
    // {
    //     selectedQuantity = Mathf.Min(selectedQuantity + 1, maxQuantity);
    //     quantityDialog?.UpdateQuantity(selectedQuantity);

    // }

    // // (4) 수량 – 버튼
    // public void DecreaseQuantity()
    // {
    //     selectedQuantity = Mathf.Max(selectedQuantity - 1, 1);
    //     quantityDialog?.UpdateQuantity(selectedQuantity);

    // }

    // (5) 판매 확정 버튼 클릭
    public void ConfirmSell()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null) return;

        // 1) 판매 정보 미리 저장
        var itemData  = selectedSlot.currentItem;
        int gain      = itemData.price;

        // 2) 돈 입금
        moneyManager.AddMoney(gain);

        // 3) 인벤토리에서 해당 아이템 1개만 제거 (슬롯은 유지)
        if (selectedSlot.originalInventorySlot != null)
        {
            var originalSlot = selectedSlot.originalInventorySlot;
            
            if (originalSlot.currentItemCount > 1)
            {
                // 여러 개가 있으면 1개만 감소
                originalSlot.currentItemCount--;
                
                // 수량 텍스트 업데이트
                if (originalSlot.countText != null)
                {
                    originalSlot.countText.text = originalSlot.currentItemCount.ToString();
                }
                
            }
            else
            {
                // 1개만 있으면 슬롯 내용 완전 제거
                inventoryManager.ClearSlotContents(originalSlot);
            }
        }

        // 4) SellUI 슬롯 새로고침 (SellUI는 열린 상태 유지)
        RefreshSellSlots();
        
        // 5) 선택 상태만 초기화 (SellUI는 닫지 않음)
        selectedSlot = null;
        sellConfirmButton.interactable = false;

        Debug.Log($"판매 완료: +{gain}G");
        
        // 6) 플레이어 돈 UI 업데이트
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"Money: {playerData.money} G";
        }
    }

    private void ResetSaleState()
    {
        // if (quantityDialog != null)
        // {
        //     Destroy(quantityDialog.gameObject);
        //     quantityDialog = null;
        // }

        selectedSlot = null;
        // selectedQuantity = 0;
        sellConfirmButton.interactable = false;
    }


    public void ShowSellUI()
    {
        OnSellButtonClicked();
    }

    public void CancelSell() 
    {
        sellUI.SetActive(false);
        ResetSaleState();
    }
}