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

    private void Awake()
    {
        // Inspector에서 할당되지 않은 경우 싱글톤 사용
        if (buySystem == null)
        {
            // 싱글톤 Instance 먼저 확인
            if (BuySystem.Instance != null)
            {
                buySystem = BuySystem.Instance;
                Debug.Log($"ShopItem: BuySystem 싱글톤을 사용합니다: {buySystem.gameObject.name}");
            }
            else
            {
                // 싱글톤이 없으면 직접 찾기
                buySystem = FindFirstObjectByType<BuySystem>();
                
                // 비활성화된 것도 포함해서 찾기
                if (buySystem == null)
                {
                    buySystem = FindFirstObjectByType<BuySystem>(FindObjectsInactive.Include);
                }
                
                if (buySystem == null)
                {
                    Debug.LogError("ShopItem: BuySystem을 찾을 수 없습니다! 씬에 BuySystem이 있는지 확인하세요.");
                    Debug.LogError("해결방법: 1) BuySystem GameObject가 활성화되어 있는지 확인 2) Inspector에서 직접 할당");
                }
                else
                {
                    Debug.Log($"ShopItem: BuySystem을 자동으로 찾았습니다: {buySystem.gameObject.name}");
                }
            }
        }

        if (shopItemData == null)
            shopItemData = GetComponent<ShopItemData>(); // 또는 직접 생성하거나 리소스에서 로드
    }

    // 외부에서 호출할 수 있는 구매 메서드 (ItemRaycast 호환성)
    public void PickupItem()
    {
        if (shopItemData != null)
        {
            if (buySystem != null)
            {
                buySystem.AddToShopItemCart(shopItemData);
                Debug.Log($"{shopItemData.itemName}이(가) 장바구니에 추가되었습니다!");
            }
            else
            {
                Debug.LogWarning("BuySystem이 연결되지 않았습니다!");
            }
        }
        else
        {
            Debug.LogWarning("ShopItemData가 설정되지 않았습니다!");
        }
    }
}