using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public ItemData itemData;         // 이 오브젝트의 아이템 정보
    public BuySystem buySystem;       // BuySystem 컴포넌트 참조

    // 예: 마우스 클릭으로 아이템을 집는 경우
    private void OnMouseDown()
    {
        if (buySystem != null && itemData != null)
        {
            buySystem.AddToCart(itemData);
            Debug.Log(itemData.itemName + "이(가) 장바구니에 추가되었습니다.");
        }
    }
}