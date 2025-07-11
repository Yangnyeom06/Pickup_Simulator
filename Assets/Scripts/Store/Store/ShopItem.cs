using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public ShopItemData shopItemData;
    public BuySystem buySystem;

    private void OnMouseDown()
    {
        if (buySystem != null && shopItemData != null)
        {
            buySystem.AddToCart(shopItemData);
            Debug.Log($"{shopItemData.itemName}이(가) 장바구니에 추가되었습니다.");
        }
        else
        {
            Debug.LogWarning("BuySystem, ShopItemData가 비어 있습니다.");
        }
    }
}