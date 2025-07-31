
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public PlayerData playerData;
    public MoneyManager moneyManager;
    public GameObject cartDialog; // 장바구니 UI 프리팹

    // 장바구니에 있는 아이템이 담긴 딕셔너리
    private Dictionary<ShopItemData, int> cartItems = new Dictionary<ShopItemData, int>();
    private Dictionary<SnackData, int> cartSnacks = new Dictionary<SnackData, int>();
    
    // UI 갱신 플래그
    private bool needsUIRefresh = true;
		
    // 아이템을 장바구니에 추가하는 메서드
    public void AddToShopItemCart(ShopItemData shopItemData)
    {
        // cartItems 딕셔너리 안에 해당 item이 있는지 확인
        if (cartItems.ContainsKey(shopItemData))
        {
            // 있으면 수량 증가
            cartItems[shopItemData] ++; 
        }
        else
        {
            // 없으면 새로 추가
            cartItems.Add(shopItemData, 1);
        }
        // 장바구니 UI 업데이트
        UpdateCartUI();
    }

    public void AddToSnackCart(SnackData snackData)
    {
        if (snackData == null)
        {
            Debug.LogError($"CartSnack is null");
            return;
        }
        
        Debug.Log($"AddToSnackCart 시작 - 스낵: {snackData.snackName} (ID: {snackData.itemID})");
        Debug.Log($"현재 cartSnacks.ContainsKey 결과: {cartSnacks.ContainsKey(snackData)}");
        
        if (cartSnacks.ContainsKey(snackData))
        {
            cartSnacks[snackData] ++;
            Debug.Log($"기존 스낵 수량 증가: {snackData.snackName} -> {cartSnacks[snackData]}");
        }
        else
        {
            cartSnacks.Add(snackData, 1);
            Debug.Log($"새 스낵 추가: {snackData.snackName} -> 1");
        }

        needsUIRefresh = true; // UI 갱신 필요 표시
        Debug.Log($"AddToSnackCart 완료 - 총 cartSnacks 개수: {cartSnacks.Count}");
        
        // CartUI가 열려있을 때만 즉시 갱신
        if (cartDialog != null && cartDialog.activeSelf)
        {
            UpdateCartUI();
        }
    }

    // 장바구니 총 가격 계산
    private int CalculateTotalPrice()
    {
        int total = 0;
        
        // 일반 아이템 가격 계산
        foreach (var item in cartItems)
        {
            total += item.Key.price * item.Value;
        }
        
        // 스낵 가격 계산
        foreach (var snack in cartSnacks)
        {
            total += snack.Key.price * snack.Value;
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
            Debug.LogError("cartDialog is not assigned!");
            return;
        }
        
        Debug.Log("CartUI 열기");
        cartDialog.SetActive(true);
        needsUIRefresh = true; // CartUI를 열 때 갱신 필요
        UpdateCartUI();
    }

    public void OpenCartWithInventorySnacks()
    {
        // 안전성 검사 추가
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is null!");
            return;
        }

        if (cartDialog == null)
        {
            Debug.LogError("cartDialog is not assigned! Please assign the cart dialog GameObject in the inspector.");
            return;
        }

        cartSnacks.Clear(); // 기존 장바구니 초기화

        List<SnackData> snacks = InventoryManager.Instance.GetPickedUpSnacks();
        if (snacks != null)
        {
            foreach (SnackData snack in snacks)
            {
                if (snack != null)
                {
                    AddToSnackCart(snack);
                }
            }
        }
        else
        {
            Debug.LogWarning("GetPickedUpSnacks() returned null!");
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
        Debug.Log($"UpdateCartUI - cartSnacks 개수: {cartSnacks.Count}, needsUIRefresh: {needsUIRefresh}");
        
        // cartSnacks 내용 상세 로그
        foreach (var snack in cartSnacks)
        {
            Debug.Log($"Cart에 있는 스낵: {snack.Key.snackName} (ID: {snack.Key.itemID}) x{snack.Value}");
        }

        // 장바구니가 비어있는지 확인
        bool isEmpty = (cartItems.Count == 0 && cartSnacks.Count == 0);
        
        // Empty 메시지 표시/숨김
        emptyCartMessage.SetActive(isEmpty);

        // UI 갱신이 필요할 때만 슬롯 재생성
        if (needsUIRefresh)
        {
            // 기존 슬롯 모두 삭제
            foreach (Transform child in cartContentParent)
            {
                Destroy(child.gameObject);
            }

            if (!isEmpty)
            {
                // 스낵 슬롯 생성
                foreach (var snack in cartSnacks)
                {
                    GameObject slotObj = Instantiate(cartItemSlot, cartContentParent);
                    CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();

                    if (slot != null)
                    {
                        slot.SnackSetup(snack.Key, snack.Value, this);
                        Debug.Log($"스낵 슬롯 생성: {snack.Key.snackName} (ID: {snack.Key.itemID}) x{snack.Value}");
                    }
                }

                // 일반 아이템 슬롯 생성
                foreach (var item in cartItems)
                {
                    GameObject slotObj = Instantiate(cartItemSlot, cartContentParent);
                    CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();
                    slot.ItemSetup(item.Key, item.Value, this);
                }
            }
            
            needsUIRefresh = false; // 갱신 완료
        }

        // 총 가격 계산 및 표시 (매번 업데이트)
        int totalPrice = CalculateTotalPrice();
        totalPriceText.text = $"Total Price: {totalPrice} G";

        // 구매 버튼 활성화 조건 (매번 업데이트)
        int playerMoney = playerData.money;
        purchaseButton.interactable = (!isEmpty && totalPrice <= playerMoney);

        // 플레이어 소지금액 표시 (매번 업데이트)
        playerMoneyText.text = $"Money: {playerMoney} G";

        Debug.Log($"Cart 상태 - 스낵: {cartSnacks.Count}, 아이템: {cartItems.Count}, 총 가격: {totalPrice}, 비어있음: {isEmpty}");
    }

    // 구매 버튼 클릭 시 호출될 메서드
    public void OnPurchaseButtonClicked()
    {  
        // 총 가격 계산 메소드 호출
        int totalPrice = CalculateTotalPrice();
        int playerMoney = playerData.money;
        
        // 아이템의 총 가격이 플레이어의 소지금액보다 적을 때
        if (totalPrice <= playerMoney)
        {
            // 돈 지불
            moneyManager.SpendMoney(totalPrice);

            // 남은 아이템을 저장할 딕셔너리
            Dictionary<ShopItemData, int> remainingItems = new Dictionary<ShopItemData, int>();
            Dictionary<SnackData, int> remainingSnacks = new Dictionary<SnackData, int>();
            
            // 일반 아이템들을 인벤토리에 추가
            foreach (var item in cartItems) // 장바구니에 있는 아이템과 수량을 저장하는 딕셔너리
            {
                // item.Value : 장바구니에 담긴 아이템 수량
                int remaining = item.Value;

                // 남은 수량만큼 인벤토리에 추가 시도
                for (int i = 0; i < remaining; i++)
                {
                    // 현재 장바구니에서 꺼낸 아이템(item.Key)을 인벤토리에 넣어보고, 넣기에 성공했는지 여부를 added에 저장
                    bool added = InventoryManager.Instance.AddShopItem(item.Key);

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
                // 남은 수량이 있으면 장바구니에 남김
                if (remaining > 0)
                {
                    remainingItems[item.Key] = remaining;
                }
            }

            // cartSnacks에 있는 모든 스낵을 인벤토리에 추가
            foreach (var snack in cartSnacks)
            {
                int remaining = snack.Value;
                
                // 남은 수량만큼 인벤토리에 추가 시도
                for (int i = 0; i < remaining; i++)
                {
                    bool added = InventoryManager.Instance.AddSnack(snack.Key);
                    
                    if (added)
                    {
                        remaining--;
                    }
                    else
                    {
                        break; // 인벤토리가 가득 차면 더 이상 시도하지 않음
                    }
                }
                
                // 남은 수량이 있으면 장바구니에 남김
                if (remaining > 0)
                {
                    remainingSnacks[snack.Key] = remaining;
                }
            }

            // 장바구니 갱신
            cartItems = remainingItems;
            cartSnacks = remainingSnacks;
            needsUIRefresh = true; // 구매 후 UI 갱신 필요
            UpdateCartUI();

            if (cartItems.Count == 0 && cartSnacks.Count == 0)
            {
                Debug.Log("구매 완료! 모든 아이템이 인벤토리에 추가되었습니다.");
            }
            else
            {
                Debug.Log("인벤토리가 가득차서 일부 아이템은 장바구니에 남아있습니다.");
            }
        }
        else
        {
            Debug.Log("돈이 부족합니다");
        }
    }

    // 장바구니에서 스낵 삭제
    public void DeleteSnackFromCart(SnackData snackData)
    {
        if (snackData == null)
        {
            Debug.LogWarning("삭제하려는 SnackData가 null입니다!");
            return;
        }

        if (cartSnacks.ContainsKey(snackData))
        {
            cartSnacks[snackData]--;
            Debug.Log($"{snackData.snackName} 수량 감소: {cartSnacks[snackData]}");

            // 수량이 0이 되면 완전히 제거
            if (cartSnacks[snackData] <= 0)
            {
                cartSnacks.Remove(snackData);
                Debug.Log($"{snackData.snackName}이(가) 장바구니에서 완전히 제거되었습니다.");
            }

            needsUIRefresh = true; // UI 갱신 필요
            UpdateCartUI();
        }
        else
        {
            Debug.LogWarning($"{snackData.snackName}이(가) 장바구니에 없습니다!");
        }
    }

    // 장바구니에서 일반 아이템 삭제
    public void DeleteShopItemFromCart(ShopItemData shopItemData)
    {
        if (shopItemData == null)
        {
            Debug.LogWarning("삭제하려는 ShopItemData가 null입니다!");
            return;
        }

        if (cartItems.ContainsKey(shopItemData))
        {
            cartItems[shopItemData]--;
            Debug.Log($"{shopItemData.itemName} 수량 감소: {cartItems[shopItemData]}");

            // 수량이 0이 되면 완전히 제거
            if (cartItems[shopItemData] <= 0)
            {
                cartItems.Remove(shopItemData);
                Debug.Log($"{shopItemData.itemName}이(가) 장바구니에서 완전히 제거되었습니다.");
            }

            needsUIRefresh = true; // UI 갱신 필요
            UpdateCartUI();
        }
        else
        {
            Debug.LogWarning($"{shopItemData.itemName}이(가) 장바구니에 없습니다!");
        }
    }

    // 장바구니 닫기
    public void CloseCartDialog()
    {
        if (cartDialog != null)
        {
            cartDialog.SetActive(false);
        }
        else
        {
            Debug.LogError("cartDialog is not assigned! Please assign the cart dialog GameObject in the inspector.");
        }
    }
}
