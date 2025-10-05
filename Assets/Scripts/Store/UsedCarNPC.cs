using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UsedCarNPC : MonoBehaviour, ISaleSystem
{
    public Camera mainCamera;
    public float rayDistance = 100f;

    // 현재 선택된 슬롯·수량
    private InventorySlotData selectedSlot;
    // public int selectedQuantity { get; set; } = 0;
    // private QuantityDialog    quantityDialog;
    
    // 중복 판매 방지 플래그    
    private bool isSelling = false;

    [Header("UI References")]
    public GameObject sellUI;                   // Sell 모드 전체 패널
    public GameObject slotPrefab;               // 슬롯 프리팹 (InventorySlotData 컴포넌트 포함)
    public Transform  slotParent;               // 슬롯이 붙을 부모 (Layout Group 등)
    public Button     sellConfirmButton;        // 최종 판매 확정 버튼

    [Header("Managers")]
    public MoneyManager     moneyManager;
    public InventoryManager inventoryManager;
    
    [Header("Player References")]
    public ItemRaycast itemRaycast;
    
    [Header("Price Settings")]
    [SerializeField] private float priceMultiplier = 1.5f; // 중고트럭에서 더 비싸게 팔 수 있는 배수

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
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rayDistance))
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
        // 필수 참조 체크
        if (slotParent == null || slotPrefab == null)
        {
            Debug.LogError($"[UsedCarNPC] 필수 참조가 null입니다! slotParent: {slotParent}, slotPrefab: {slotPrefab}");
            return;
        }
        
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
                Debug.LogWarning("[UsedCarNPC] ItemRaycast를 찾을 수 없습니다!");
                return;
            }
        }
        
        // 현재 들고 있는 Large 아이템 확인
        if (itemRaycast.IsHoldingLargeItem && itemRaycast.CurrentLargeItemData != null)
        {
            var item = itemRaycast.CurrentLargeItemData.itemData;
            
            if (CanSell(item))
            {
                var go = Instantiate(slotPrefab, slotParent);
                var ui = go.GetComponentInChildren<InventorySlotData>();
                
                if (ui == null)
                {
                    return;
                }
    
                ui.SetupSlot(item, 1, this); // 들고 있는 Large 아이템 1개
                ui.originalInventorySlot = null; // 인벤토리 슬롯이 아니므로 null
                
                Debug.Log($"[UsedCarNPC] 슬롯 생성 완료: {item.itemName}");
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

        // 기존 다이얼로그 제거
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
            // this,                     // SaleSystem 인스턴스
            // slot.currentItem,         // 판매할 아이템 정보
            // slot.currentItemCount     // 최대 선택 가능한 수량
        // );

        // 판매 확정 버튼 활성화 등…
        sellConfirmButton.interactable = true;

    }

    public bool CanSell(ItemData item)
    {
        return item != null && item.itemType == ItemType.Large;
    }

    // public void IncreaseQuantity(int maxQty)
    // {
    //     selectedQuantity = Mathf.Min(selectedQuantity + 1, maxQty);
    //     // quantityDialog?.UpdateQuantity(selectedQuantity);
    // }

    // public void DecreaseQuantity()
    // {
    //     selectedQuantity = Mathf.Max(selectedQuantity - 1, 1);
    //     // quantityDialog?.UpdateQuantity(selectedQuantity);
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
        
        // 더러움 상태에 따른 가격 조정
        int cleanPrice = (int)(itemData.price * itemData.dirty); // 더러움 페널티 적용된 가격
        int gain = Mathf.RoundToInt(cleanPrice * priceMultiplier); // 중고트럭 프리미엄 적용
        
        // 더러움 상태 및 가격 정보 로그
        int basePrice = itemData.price; // price 기준으로 변경
        // int penalty = itemData.GetDirtyPenalty();
        Debug.Log($"중고차 판매: {itemData.itemName}");
        // Debug.Log($"  - 더러움 상태: {itemData.GetDirtyStateString()} (dirty: {itemData.dirty:F2})");
        Debug.Log($"  - 기본 가격: {basePrice}G");
        // Debug.Log($"  - 더러움 페널티: -{penalty}G");
        Debug.Log($"  - 페널티 적용 후: {cleanPrice}G");
        Debug.Log($"  - 프리미엄 배수: {priceMultiplier}x");
        Debug.Log($"  - 최종 판매가: {gain}G");

        // 2) 돈 입금
        moneyManager.AddMoney(gain);

        // 3) 현재 들고 있는 Large 아이템 제거
        bool itemRemoved = false;
        if (!itemRaycast.IsHoldingLargeItem)
        {
            Debug.LogError("[UsedCarNPC] itemRaycast가 null입니다! Inspector에서 할당하세요.");
        }
        else if (!itemRaycast.IsHoldingLargeItem)
        {
            Debug.LogWarning("[UsedCarNPC] 현재 들고 있는 Large 아이템이 없습니다. (판매 전 상태 확인)");
        }
        else
        {
            itemRaycast.SellCurrentLargeItem();
            itemRemoved = true;
        }
        
        Debug.Log($"중고차 판매: {itemData.itemName} (기본가격: {basePrice}G → 판매가격: {gain}G, 배수: {priceMultiplier}x)");

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
        yield return new WaitForSeconds(0.5f);

        sellUI.SetActive(false);

        int playerMoney = PlayerManager.Instance.money;
        Debug.Log($"판매 완료: +{gain}G");
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"Money: {playerMoney} G";
        }
        
        // 판매 완료 후 플래그 리셋
        isSelling = false;
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
