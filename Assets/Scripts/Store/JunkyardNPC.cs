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

    [SerializeField] private TMP_Text playerMoneyText;

    void Start()
    {
        // 판매 확정 버튼 이벤트 설정
        if (sellConfirmButton != null)
        {
            sellConfirmButton.onClick.RemoveAllListeners();
            sellConfirmButton.onClick.AddListener(() => {
                Debug.Log("[JunkyardNPC] 판매 확정 버튼이 클릭되었습니다!");
                ConfirmSell();
            });
            sellConfirmButton.interactable = false; // 시작시에는 비활성화
            Debug.Log("[JunkyardNPC] 판매 확정 버튼 이벤트 설정 완료!");
        }
        else
        {
            Debug.LogError("[JunkyardNPC] sellConfirmButton이 null입니다! Inspector에서 할당하세요!");
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
                Debug.LogWarning("[JunkyardNPC] ItemRaycast를 찾을 수 없습니다!");
                return;
            }
        }
        
        // 현재 들고 있는 Large 아이템 확인
        if (itemRaycast.IsHoldingLargeItem && itemRaycast.CurrentLargeItemData != null)
        {
            var item = itemRaycast.CurrentLargeItemData.itemData;
            
            if (CanSell(item))
            {
                Debug.Log($"[JunkyardNPC] 슬롯 생성 시작: {item.itemName}");
                var go = Instantiate(slotPrefab, slotParent);
                var ui = go.GetComponent<InventorySlotData>();
                
                if (ui == null)
                {
                    Debug.LogError("[JunkyardNPC] 생성된 슬롯에 InventorySlotData 컴포넌트가 없습니다!");
                    return;
                }
    
                ui.SetupSlot(item, 1, this); // 들고 있는 Large 아이템 1개
                ui.originalInventorySlot = null; // 인벤토리 슬롯이 아니므로 null
                
                Debug.Log($"[JunkyardNPC] 들고 있는 Large 아이템 '{item.itemName}' 판매 슬롯에 표시될");
                Debug.Log($"[JunkyardNPC] 생성된 슬롯 GameObject: {go.name}");
            }
        }
        else
        {
            // 판매 후 UI 갱신 시에는 정상적으로 아이템이 없는 상태
            if (itemRaycast != null)
            {
                Debug.Log("[JunkyardNPC] 판매 슬롯 갱신: 현재 들고 있는 Large 아이템이 없음 (정상)");
            }
            else
            {
                Debug.LogWarning("[JunkyardNPC] itemRaycast가 null입니다!");
            }
        }
    }

    // (3) 슬롯 클릭 → 다이얼로그 띄우기 + 판매 확정 버튼 활성
    public void OnSlotClicked(InventorySlotData slot)
    {
        Debug.Log($"[JunkyardNPC] OnSlotClicked() 호출됨! slot: {slot}");
        
        if (slot == null)
        {
            Debug.LogError("[JunkyardNPC] 클릭된 슬롯이 null입니다!");
            return;
        }
        
        Debug.Log($"[JunkyardNPC] slot.currentItem: {slot.currentItem}");
        Debug.Log($"[JunkyardNPC] slot.currentItem이 null인가? {slot.currentItem == null}");
        
        if (slot.currentItem == null)
        {
            Debug.LogError("[JunkyardNPC] 클릭된 슬롯의 currentItem이 null입니다!");
            Debug.LogError($"[JunkyardNPC] 슬롯 상태 - currentShopItem: {slot.currentShopItem}, currentSnackItem: {slot.currentSnackItem}");
            return;
        }

        // 대형 아이템 여부 재확인
        if (slot.currentItem.itemType != ItemType.Large)
        {
            Debug.Log("[JunkyardSystem] 대형 아이템만 판매할 수 있습니다.");
            return;
        }

        Debug.Log($"[JunkyardNPC] selectedSlot 설정 전 - 현재 selectedSlot: {selectedSlot}");
        selectedSlot = slot;
        Debug.Log($"[JunkyardNPC] 선택 완료! selectedSlot: {selectedSlot}, 아이템: {slot.currentItem.itemName}");

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
        Debug.Log($"[JunkyardNPC] 판매 확정 버튼 활성화됨! 버튼 연결 상태: {sellConfirmButton != null}");

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
        Debug.Log("[JunkyardNPC] ConfirmSell() 메서드 호출됨!");
        Debug.Log("[JunkyardNPC] selectedSlot 상태: " + selectedSlot);
        Debug.Log("[JunkyardNPC] currentItem 상태: " + (selectedSlot != null ? selectedSlot.currentItem : "null"));
        
        if (selectedSlot == null)
        {
            Debug.LogError("[JunkyardNPC] selectedSlot이 null입니다! 슬롯을 선택하지 않았거나 OnSlotClicked가 호출되지 않았습니다.");
            return;
        }
        
        if (selectedSlot.currentItem == null)
        {
            Debug.LogError("[JunkyardNPC] selectedSlot.currentItem이 null입니다! 슬롯에 아이템이 없습니다.");
            return;
        }
        
        Debug.Log("[JunkyardNPC] 조건 검사 통과! 판매 진행합니다...");

        // 1) 판매 정보 미리 저장
        var itemData  = selectedSlot.currentItem;
        int gain      = itemData.price;

        // 2) 돈 입금
        moneyManager.AddMoney(gain);

        // 3) 현재 들고 있는 Large 아이템 제거
        bool itemRemoved = false;
        
        if (itemRaycast == null)
        {
            Debug.LogError("[JunkyardNPC] itemRaycast가 null입니다! Inspector에서 할당하세요.");
        }
        else if (!itemRaycast.IsHoldingLargeItem)
        {
            Debug.LogWarning("[JunkyardNPC] 현재 들고 있는 Large 아이템이 없습니다. (판매 전 상태 확인)");
        }
        else
        {
            Debug.Log($"[JunkyardNPC] Large 아이템 제거 시작 - 현재 상태: {itemRaycast.IsHoldingLargeItem}");
            itemRaycast.SellCurrentLargeItem();
            itemRemoved = true;
            Debug.Log($"[JunkyardNPC] Large 아이템 제거 완료 - 현재 상태: {itemRaycast.IsHoldingLargeItem}");
        }
        
        Debug.Log($"쓰레기장 판매 완료: {itemData.itemName}");

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

        Debug.Log($"판매 완료: +{gain}G");
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"Money: {playerData.money} G";
        }
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
}
