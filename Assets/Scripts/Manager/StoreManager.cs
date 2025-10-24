using UnityEngine;
using UnityEngine.UI;

// 장바구니 아이템 증가/감소 버튼에 대한 스크립트
public class StoreManager : MonoBehaviour
{
    public Button plusButton; // UnityEngine.UI 버튼 컴포넌트
    public Button minusButton; // UnityEngine.UI 버튼 컴포넌트
    public ShopItemData shopItemData;
    public BuySystem buySystem;

    void Start()
    {  
        // 버튼 클릭 시 장바구니 아이템 증가/감소
        //plusButton.onClick.AddListener(() => buySystem.AdjustItemQuantity(shopItemData, 1)); // 장바구니 UI에서 plusButton 구현
        //minusButton.onClick.AddListener(() => buySystem.AdjustItemQuantity(shopItemData, -1)); // 장바구니 UI에서 minusButton 구현
    }
}