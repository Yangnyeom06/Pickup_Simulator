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

    /// <summary>
    /// 레이캐스트 거리
    /// </summary>
    [SerializeField] private float mRayDistance;


    public ShopItemData shopItemData;
    public BuySystem buySystem;


    private void TryBuyItem()
    {
        if (Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, mRayDistance))
        {
            if (Input.GetKeyDown(KeyCode.E)) 
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
        
    }
}