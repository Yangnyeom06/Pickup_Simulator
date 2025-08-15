using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

// Buy버튼에 적용
public class BuySystem : MonoBehaviour, IPointerClickHandler
{
    public PlayerData playerData;
    public MoneyManager moneyManager;
    public GameObject cartDialog; // 장바구니 UI 프리팹

    // 장바구니에 있는 아이템이 담긴 딕셔너리
    private Dictionary<ShopItemData, int> cartItems = new Dictionary<ShopItemData, int>();
		
    // 아이템을 장바구니에 추가하는 메서드
    public void AddToCart(ShopItemData shopItemData)
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

    // 장바구니에서 아이템 수량 조절
    public void AdjustItemQuantity(ShopItemData shopItemData, int amount)
    {
        if (cartItems.ContainsKey(shopItemData))
        {
            // 수량 증가
            cartItems[shopItemData] += amount;
            // 수량이 0이면 장바구니에서 제거
            if (cartItems[shopItemData] <= 0)
            {
                cartItems.Remove(shopItemData);
            }
        }
        UpdateCartUI();
    }

    // 장바구니 총 가격 계산
    private int CalculateTotalPrice()
    {
        int total = 0;
        foreach (var item in cartItems)
        {
            total += item.Key.price * item.Value;
        }
        return total;
    }

    // NPC 클릭 시 호출될 메서드
    public void OnPointerClick(PointerEventData eventData)
    {
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
        // 기존 슬롯 모두 삭제
        foreach (Transform child in cartContentParent)
        {
            Destroy(child.gameObject);
        }

        // 장바구니 아이템마다 슬롯 생성
        foreach (var item in cartItems)
        {
            GameObject slotObj = Instantiate(cartItemSlot, cartContentParent);
            CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();
            slot.Setup(item.Key, item.Value, this);
        }

        // 총 아이템 금액 구현
        int totalPrice = CalculateTotalPrice();
        totalPriceText.text = $"Total Price: {totalPrice} G";

        // 구매 버튼 구현
        int playerMoney = playerData.money;
        purchaseButton.interactable = (cartItems.Count > 0) && (totalPrice <= playerMoney);
        emptyCartMessage.SetActive(cartItems.Count == 0);

        // 플레이어 소지금액 구현
        playerMoneyText.text = $"Money: {playerMoney} G";

        Debug.Log($"cartItems.Count: {cartItems.Count}, totalPrice: {totalPrice}, playerMoney: {playerMoney}");
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
            
            // 아이템들을 인벤토리에 추가
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
            // 장바구니 갱신
            cartItems = remainingItems;
            UpdateCartUI();

            if (cartItems.Count == 0)
            {
                Debug.Log("구매 완료");
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

    // 장바구니 닫기
    public void CloseCartDialog()
    {
        cartDialog.SetActive(false);
    }
}
