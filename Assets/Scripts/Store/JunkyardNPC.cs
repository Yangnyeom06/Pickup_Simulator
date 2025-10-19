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

    [Header("UI References")]
    public GameObject sellUI;                   // Sell 모드 전체 패널
    public GameObject slotPrefab;               // 슬롯 프리팹 (InventorySlotData 컴포넌트 포함)
    public Transform  slotParent;               // 슬롯이 붙을 부모 (Layout Group 등)
    public Button     sellConfirmButton;        // 최종 판매 확정 버튼
    // public GameObject quantityDialogPrefab;     // QuantityDialog 프리팹
    // public Transform  quantityDialogParent;     // 다이얼로그를 붙일 부모

    [Header("Managers")]
    public MoneyManager     moneyManager;
    public InventoryManager inventoryManager;
    
    [Header("Player References")]
    public ItemRaycast itemRaycast;

    // 현재 선택된 슬롯·수량
    private InventorySlotData selectedSlot;
    // public int selectedQuantity { get; set; } = 0;
    // private QuantityDialog    quantityDialog;
    
    // 중복 판매 방지 플래그
    private bool isSelling = false;

    [SerializeField] private TMP_Text playerMoneyText;

    void Start()
    {
        // 판매 확정 버튼 이벤트 설정
        if (sellConfirmButton != null)
        {
            sellConfirmButton.onClick.RemoveAllListeners();
            sellConfirmButton.onClick.AddListener(() => {
                ConfirmSell();
            });
            sellConfirmButton.interactable = false; // 시작시에는 비활성화
        }
        
        // SellUI 초기 비활성화
        if (sellUI != null)
        {
            sellUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward ,out hit, rayDistance))
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
        ResetSaleState();
        RefreshSellSlots();
        UpdatePlayerMoneyUI(); // UI가 열릴 때 현재 돈 표시
    }


    /// (2) 현재 들고 있는 Large 아이템 → SellUI 슬롯으로 표시
    public void RefreshSellSlots()
    {
        // 기존 슬롯 전부 삭제
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        
        // ItemRaycast 참조 확인
        if (itemRaycast == null)
        {
            itemRaycast = FindFirstObjectByType<ItemRaycast>();
            if (itemRaycast == null)
            {
                return;
            }
        }
        
        // 현재 들고 있는 Large 아이템 확인
        if (itemRaycast.IsHoldingLargeItem && itemRaycast.CurrentLargeItemData != null)
        {
            var item = itemRaycast.CurrentLargeItemData.itemData;
            
            // item null 체크 추가
            if (item == null)
            {
                Debug.LogWarning("[JunkyardNPC] CurrentLargeItemData.itemData가 null입니다!");
                return;
            }
            
            if (CanSell(item))
            {
                var go = Instantiate(slotPrefab, slotParent);
                var ui = go.GetComponentInChildren<InventorySlotData>();
                
                if (ui == null)
                {
                    Debug.LogWarning("[JunkyardNPC] InventorySlotData 컴포넌트를 찾을 수 없습니다!");
                    return;
                }
    
                ui.SetupSlot(item, 1, this); // 들고 있는 Large 아이템 1개
                ui.originalInventorySlot = null; // 인벤토리 슬롯이 아니므로 null
            }
        }
    }

    // (3) 슬롯 클릭 → 다이얼로그 띄우기 + 판매 확정 버튼 활성
    public void OnSlotClicked(InventorySlotData slot)
    {
        
        if (slot == null)
        {
            return;
        }
        
        if (slot.currentItem == null)
        {
            return;
        }

        // 대형 아이템 여부 재확인
        if (slot.currentItem.itemType != ItemType.Large)
        {
            return;
        }

        selectedSlot = slot;

        // if (quantityDialog != null)
        // {
        //     Destroy(quantityDialog.gameObject);
        //     quantityDialog = null;
        // }

        // quantityDialogPrefab.SetActive(true);
        // var dlgGO = Instantiate(quantityDialogPrefab, quantityDialogParent, false);
        // quantityDialog = dlgGO.GetComponent<QuantityDialog>();

        // quantityDialog.Setup(
        //     this,
        //     slot.currentItem,
        //     slot.currentItemCount
        // );

        sellConfirmButton.interactable = true;

    }

    public bool CanSell(ItemData item)
    {
        return item != null && item.itemType == ItemType.Large;
    }

    // public void IncreaseQuantity(int maxQty)
    // {
    //     selectedQuantity = Mathf.Min(selectedQuantity + 1, maxQty);
    //     quantityDialog?.UpdateQuantity(selectedQuantity);
    // }

    // public void DecreaseQuantity()
    // {
    //     selectedQuantity = Mathf.Max(selectedQuantity - 1, 1);
    //     quantityDialog?.UpdateQuantity(selectedQuantity);
    // }

    // 판매 버튼 클릭 시 호출
    public void ConfirmSell()
    {
        // 중복 호출 방지
        if (isSelling)
        {
            return;
        }
        
        isSelling = true;
        
        
        if (selectedSlot == null)
        {
            isSelling = false; // 플래그 리셋
            return;
        }
        
        if (selectedSlot.currentItem == null)
        {
            isSelling = false; // 플래그 리셋
            return;
        }
        

        // 1) 판매 정보 미리 저장
        var itemData  = selectedSlot.currentItem;
        
        // 더러움 상태에 따른 가격 조정 (고물상에서는 기본 가격 기준)
        int gain = (int)(itemData.price * itemData.dirty); // 더러움 페널티 적용된 가격
        
        // 더러움 상태 및 가격 정보 로그
        int basePrice = itemData.price; // price 기준으로 변경
        int penalty = basePrice - gain;
        Debug.Log($"고물상 판매: {itemData.itemName}");
        Debug.Log($"  - 기본 가격: {basePrice}G");
        Debug.Log($"  - 더러움 페널티: -{penalty}G");
        Debug.Log($"  - 최종 판매가: {gain}G");

        // 2) 돈 입금
        moneyManager.AddMoney(gain);

        // 3) 현재 들고 있는 Large 아이템 제거
        bool itemRemoved = false;
        

        if (itemRaycast.IsHoldingLargeItem)
        {
            itemRaycast.SellCurrentLargeItem();
            itemRemoved = true;
        }

        // 4) 판매 완료 후 UI 처리
        StartCoroutine(HandlePostSaleUI(gain, itemRemoved));
    }

    private System.Collections.IEnumerator HandlePostSaleUI(int gain, bool itemWasRemoved)
    {
        // 아이템이 제거되었다면 잠시 기다려서 UI가 갱신되도록 함
        if (itemWasRemoved)
        {
            yield return new WaitForEndOfFrame();
        }

        // 5) UI 갱신 (빈 슬롯으로 표시)
        RefreshSellSlots();
        ResetSaleState();

        // 6) 잠시 기다린 후 UI 닫기 (사용자가 변화를 볼 수 있도록)

        sellUI.SetActive(false);

        int playerMoney = PlayerManager.Instance.money;
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{playerMoney} G";
        }
        
        // 판매 완료 후 플래그 리셋
        isSelling = false;
    }

    // /// <summary>
    // /// 즉시 UI 처리 (아이템이 제거되지 않은 경우)
    // /// </summary>
    // private void HandlePostSaleUIImmediate(int gain, bool itemWasRemoved)
    // {
    //     if (itemWasRemoved)
    //     {
    //         yield return new WaitForEndOfFrame();
    //     }

    //     RefreshSellSlots();
    //     ResetSaleState();
    //     sellUI.SetActive(false);

    //     Debug.Log($"판매 완료: +{gain}G");
    //     if (playerMoneyText != null)
    //     {
    //         playerMoneyText.text = $"Money: {playerData.money} G";
    //     }
    // }

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

    public void CancelSell() 
    {
        sellUI.SetActive(false);
        ResetSaleState();
    }

    private void UpdatePlayerMoneyUI()
    {
        int playerMoney = PlayerManager.Instance.money;
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"Money: {playerMoney} G";
        }
    }
}
