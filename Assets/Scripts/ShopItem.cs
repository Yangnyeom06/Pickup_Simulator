using UnityEngine;

public class ShopItem : MonoBehaviour
{

    [Header("레이캐스트를 쏠 카메라")]
    [SerializeField] private Camera mRayCamera; //레이를 쏠 카메라 (메인카메라)
    public float rayDistance = 100f;

    /// <summary>
    /// 레이캐스트 된 아이템
    /// </summary>
    private RaycastHit mHit;

    public ShopItemData shopItemData;
    public BuySystem buySystem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryBuyItem();
        }
    }


    private void TryBuyItem()
    {
        if (!Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, rayDistance)) return;

        ShopItem shopItem = mHit.transform.GetComponent<ShopItem>();
        if (shopItem == null || shopItem.shopItemData == null || shopItem.buySystem == null)
        {
            Debug.LogWarning("BuySystem, ShopItemData가 비어 있습니다.");
            return;
        }

        shopItem.buySystem.AddToShopItemCart(shopItem.shopItemData);
        Debug.Log($"{shopItem.shopItemData.itemName}이(가) 장바구니에 추가되었습니다.");
    }
}