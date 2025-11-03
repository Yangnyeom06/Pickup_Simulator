
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

// Buy버튼에 적용
public class BuySystem : MonoBehaviour, IPointerClickHandler
{
    public static BuySystem Instance { get; private set; }
    private bool isPurchasing = false; // 구매 처리 중복 방지 플래그

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public MoneyManager moneyManager;
    public GameObject cartDialog; // 장바구니 UI 프리팹

    // 장바구니에 있는 아이템이 담긴 딕셔너리 (통합)
    private Dictionary<StoreItemData, int> cartItems = new Dictionary<StoreItemData, int>();
    
    // UI 갱신 플래그
    private bool needsUIRefresh = true;
		
    // 아이템을 장바구니에 추가하는 메서드 (통합)
    public void AddToCart(StoreItemData storeItemData)
    {
        if (storeItemData == null)
        {
            return;
        }

        // cartItems 딕셔너리 안에 해당 item이 있는지 확인
        if (cartItems.ContainsKey(storeItemData))
        {
            // 있으면 수량 증가
            cartItems[storeItemData]++; 
        }
        else
        {
            // 없으면 새로 추가
            cartItems.Add(storeItemData, 1);
        }

        needsUIRefresh = true; // UI 갱신 필요 표시
        
        // CartUI가 열려있을 때만 즉시 갱신
        if (cartDialog != null && cartDialog.activeSelf)
        {
            UpdateCartUI();
        }
    }

    // 하위 호환성을 위한 메서드들
    public void AddToShopItemCart(StoreItemData shopItemData)
    {
        AddToCart(shopItemData);
    }

    public void AddToSnackCart(StoreItemData snackItemData)
    {
        AddToCart(snackItemData);
    }

    // StoreItem과 연동을 위한 통합 메서드
    public void AddStoreItemToCart(StoreItem storeItem)
    {
        if (storeItem == null)
        {
            Debug.LogWarning("StoreItem이 null입니다!");
            return;
        }

        if (storeItem.storeItemData != null)
        {
            AddToCart(storeItem.storeItemData);
        }
        else
        {
            Debug.LogWarning("StoreItemData가 설정되지 않았습니다!");
        }
    }

    // 장바구니 총 가격 계산
    private int CalculateTotalPrice()
    {
        int total = 0;
        
        // 모든 아이템 가격 계산 (통합)
        foreach (var item in cartItems)
        {
            total += item.Key.price * item.Value;
        }
        
        return total;
    }

    // CartUI 열기
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenCart();
    }

    // 장바구니 열기 (공통 메서드)
    public void OpenCart()
    {
        if (cartDialog == null)
        {
            return;
        }
        
        cartDialog.SetActive(true);
        needsUIRefresh = true; // CartUI를 열 때 갱신 필요
        UpdateCartUI();
    }

    public void OpenCartWithInventorySnacks()
    {
        // 안전성 검사 추가
        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (cartDialog == null)
        {
            return;
        }

        // 기존 스낵 아이템들만 제거 (상점 아이템은 유지)
        var itemsToRemove = new List<StoreItemData>();
        foreach (var item in cartItems)
        {
            if (item.Key.dataType == StoreItemDataType.Snack)
            {
                itemsToRemove.Add(item.Key);
            }
        }
        foreach (var item in itemsToRemove)
        {
            cartItems.Remove(item);
        }

        List<StoreItemData> snacks = InventoryManager.Instance.GetPickedUpSnacks();
        if (snacks != null)
        {
            foreach (StoreItemData snack in snacks)
            {
                if (snack != null)
                {
                    AddToCart(snack);
                }
            }
        }

        needsUIRefresh = true; // UI 갱신 필요
        UpdateCartUI();
        cartDialog.SetActive(true);
    }

    // StoreNPC에서 상점 아이템들을 장바구니에 추가하는 메서드
    public void OpenCartWithStoreItems(StoreItemData[] storeItems = null)
    {
        if (cartDialog == null)
        {
            return;
        }

        // 기존 장바구니 초기화하지 않고 유지

        // 인벤토리 스낵들도 추가
        if (InventoryManager.Instance != null)
        {
            List<StoreItemData> snacks = InventoryManager.Instance.GetPickedUpSnacks();
            if (snacks != null)
            {
                foreach (StoreItemData snack in snacks)
                {
                    if (snack != null)
                    {
                        AddToCart(snack);
                    }
                }
            }
        }

        // 전달받은 상점 아이템들을 장바구니에 추가 (선택사항)
        if (storeItems != null)
        {
            foreach (StoreItemData item in storeItems)
            {
                if (item != null)
                {
                    AddToCart(item);
                }
            }
        }

        needsUIRefresh = true; // UI 갱신 필요
        UpdateCartUI();
        cartDialog.SetActive(true);
    }

    // 장바구니 UI 업데이트
    [SerializeField] private Transform cartContentParent; 
    [SerializeField] private GameObject cartItemSlot;
    // 예시: 총 가격 텍스트 오브젝트
    [SerializeField] private TMP_Text totalPriceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private GameObject emptyCartMessage;
    [SerializeField] private TMP_Text playerMoneyText;

    public void UpdateCartUI()
    {
        // 장바구니가 비어있는지 확인
        bool isEmpty = (cartItems.Count == 0);
        
        // Empty 메시지 표시/숨김
        emptyCartMessage.SetActive(isEmpty);

        // UI 갱신이 필요할 때만 슬롯 재생성
        if (needsUIRefresh)
        {
            
            // 기존 슬롯 모두 삭제
            int childCount = cartContentParent.childCount;
            
            foreach (Transform child in cartContentParent)
            {
                Destroy(child.gameObject);
            }

            if (!isEmpty)
            {
                // 모든 아이템 슬롯 생성 (통합)
                foreach (var item in cartItems)
                {
                    GameObject slotObj = Instantiate(cartItemSlot, cartContentParent);
                    CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();
                    if (slot != null)
                    {
                        slot.ItemSetup(item.Key, item.Value, this);
                    }
                }
            }
            
            needsUIRefresh = false; // 갱신 완료
        }

        // 총 가격 계산 및 표시 (매번 업데이트)
        int totalPrice = CalculateTotalPrice();
        
        // 총 가격을 UI에 표시
        if (totalPriceText != null)
        {
            totalPriceText.text = $"{totalPrice} G";
        }

        // 구매 버튼 활성화 조건 (매번 업데이트)
        int playerMoney = PlayerManager.Instance.money;
        purchaseButton.interactable = (!isEmpty && totalPrice <= playerMoney);

        // 플레이어 소지금액 표시 (매번 업데이트)
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{playerMoney} G";
        }
    }

    // 구매 버튼 클릭 시 호출될 메서드
    public void OnPurchaseButtonClicked()
    {
        // 중복 호출 방지
        if (isPurchasing)
        {
            return;
        }
        
        isPurchasing = true;
        
        // 총 가격 계산 메소드 호출
        int totalPrice = CalculateTotalPrice();
        int playerMoney = PlayerManager.Instance.money;
        
        
        // 장바구니가 비어있는 경우
        if (totalPrice <= 0)
        {
            isPurchasing = false;
            return;
        }
        
        // 아이템의 총 가격이 플레이어의 소지금액보다 적을 때
        if (totalPrice <= playerMoney)
        {
            // 먼저 장바구니 총 금액만큼 돈 지불
            moneyManager.SpendMoney(totalPrice);
            
            // 남은 아이템을 저장할 딕셔너리 (인벤토리가 가득 찬 경우에만 사용)
            Dictionary<StoreItemData, int> remainingItems = new Dictionary<StoreItemData, int>();
            
            // 모든 아이템들을 인벤토리에 추가 (통합)
            foreach (var item in cartItems) // 장바구니에 있는 아이템과 수량을 저장하는 딕셔너리
            {
                // item.Value : 장바구니에 담긴 아이템 수량
                int remaining = item.Value;
                int originalRemaining = remaining; // 원래 수량 저장

                // 남은 수량만큼 인벤토리에 추가 시도
                for (int i = 0; i < originalRemaining; i++)
                {
                    bool added = false;
                    
                    // 아이템 타입에 따라 적절한 인벤토리 메서드 호출
                    if (item.Key.dataType == StoreItemDataType.ShopItem)
                    {
                        added = InventoryManager.Instance.AddShopItem(item.Key);
                    }
                    else if (item.Key.dataType == StoreItemDataType.Snack)
                    {
                        added = InventoryManager.Instance.AddSnack(item.Key);
                    }

                    // 만약 성공하면 장바구니에 담긴 아이템 수량 감소
                    if (added)
                    {
                        remaining--;
                    }
                    // 실패한 경우
                    else
                    {
                        break; // 인벤토리가 가득 차면 더 이상 시도하지 않음
                    }
                }
                
                // 구매는 이미 완료됨 (총 금액 차감 완료)
                
                // 남은 수량이 있으면 장바구니에 남김
                if (remaining > 0)
                {
                    remainingItems[item.Key] = remaining;
                }
            }
            
            // 구매 완료 (총 금액은 이미 차감됨)

            // 장바구니 갱신
            cartItems = remainingItems;
            needsUIRefresh = true; // 구매 후 UI 갱신 필요
            UpdateCartUI();

        }
        
        // 구매 처리 완료 후 플래그 해제
        isPurchasing = false;
    }

    // 장바구니에서 아이템 삭제 (통합)
    public void DeleteFromCart(StoreItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        if (cartItems.ContainsKey(itemData))
        {
            cartItems[itemData]--;

            // 수량이 0이 되면 완전히 제거
            if (cartItems[itemData] <= 0)
            {
                cartItems.Remove(itemData);
            }

            needsUIRefresh = true; // UI 갱신 필요
            UpdateCartUI();
        }
    }

    // 하위 호환성을 위한 메서드들
    public void DeleteSnackFromCart(StoreItemData snackData)
    {
        DeleteFromCart(snackData);
    }

    public void DeleteShopItemFromCart(StoreItemData shopItemData)
    {
        DeleteFromCart(shopItemData);
    }

    // 장바구니 전체 비우기
    public void ClearCart()
    {
        cartItems.Clear();
        needsUIRefresh = true;
        UpdateCartUI();
    }

    // 장바구니 닫기
    public void CloseCartDialog()
    {
        if (cartDialog != null)
        {
            cartDialog.SetActive(false);
        }
    }
}
