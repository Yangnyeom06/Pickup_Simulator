using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// 상점 NPC 오브젝트에 적용한 코드
public class BuySystem : MonoBehaviour
{
    public PlayerData playerData;
    public MoneyManager moneyManager;
    public InventorySlotData inventorySlot;
    public GameObject cartDialogPrefab;

    private Dictionary<ShopItemData, int> cartItems = new Dictionary<ShopItemData, int>();
    private GameObject cartDialog; // 현재 열린 장바구니 다이얼로그
		
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
    public void OnNPCClicked()
    {
        ShowCartDialog();
    }

    // 장바구니 다이얼로그 표시
    private void ShowCartDialog()
    {
        // 장바구니 UI가 이미 열려있는지 확인
        if (cartDialog == null)
        {
            // 없으면 생성
            cartDialog = Instantiate(cartDialogPrefab); //prefab(미리 만들어둔 UI 오브젝트 템플릿)을 복제해서 실제 게임 화면에 띄움 => UI 중복 생성 방지지
            // 장바구니 UI 초기화
            UpdateCartUI();
        }
    }

    // 장바구니 UI 업데이트
    private void UpdateCartUI()
    {
        if (cartDialog != null)
        {
            // TODO: 장바구니 UI의 아이템 목록, 수량, 가격 등을 업데이트
        }
    }

    // 구매 버튼 클릭 시 호출될 메서드
    public void OnPurchaseButtonClicked()
    {
        int totalPrice = CalculateTotalPrice();
        
        if (totalPrice <= playerData.money)
        {
            // 돈 지불
            moneyManager.SpendMoney(totalPrice);

            // 남은 아이템을 저장할 딕셔너리
            Dictionary<ShopItemData, int> remainingItems = new Dictionary<ShopItemData, int>();
            
            // 아이템들을 인벤토리에 추가
            foreach (var item in cartItems) // 장바구니에 있는 아이템과 수량을 저장하는 딕셔너리
            {
                int remaining = item.Value;

                // 남은 수량만큼 인벤토리에 추가 시도
                for (int i = 0; i < remaining; i++)
                {
                    bool added = InventoryManager.Instance.AddShopItem(item.Key);
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
        if (cartDialog != null)
        {
            Destroy(cartDialog);
            cartDialog = null;
        }
    }

    void OnMouseDown()
    {
        OnNPCClicked(); // 장바구니 UI 띄우기
    }
}
