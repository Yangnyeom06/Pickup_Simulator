using Unity.VisualScripting;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public StoreItemData storeItemData; // 통합 상점 아이템 데이터
    
    [Header("시스템 연동")]
    public BuySystem buySystem; // Inspector에서 할당할 BuySystem
    
    private InventoryManager inventoryManager;

    public enum StoreItemType
    {
        ShopItem,
        Snack
    }
    
    [Header("아이템 타입")]
    public StoreItemType itemType = StoreItemType.ShopItem;

    void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }
        
        if (buySystem == null)
        {
            buySystem = FindFirstObjectByType<BuySystem>();
        }
    }

    public void SetStoreItemData(StoreItemData data)
    {
        storeItemData = data;
        if (data != null)
        {
            itemType = data.dataType == StoreItemDataType.ShopItem ? StoreItemType.ShopItem : StoreItemType.Snack;
        }
    }

    // 하위 호환성을 위한 메서드들
    public void SetShopItemData(StoreItemData data)
    {
        SetStoreItemData(data);
    }

    public void SetSnackItemData(StoreItemData data)
    {
        SetStoreItemData(data);
    }

    public void GotItem()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }

        bool success = false;
        string itemName = "";

        if (storeItemData != null)
        {
            if (storeItemData.dataType == StoreItemDataType.ShopItem)
            {
                success = inventoryManager.AddShopItem(storeItemData);
                itemName = storeItemData.itemName;
            }
            else if (storeItemData.dataType == StoreItemDataType.Snack)
            {
                success = inventoryManager.AddSnack(storeItemData);
                itemName = storeItemData.snackName;
            }
        }

        if (!success)
        {
            Debug.LogWarning($"인벤토리가 가득 차서 {itemName}을(를) 추가할 수 없습니다!");
        }
    }

    // ItemRaycast와 호환을 위한 PickupItem 메서드 (BuySystem 연동)
    public void PickupItem()
    {
        // BuySystem을 통해 장바구니에 추가
        BuySystem buySystem = GetBuySystem();

        if (buySystem != null)
        {
            buySystem.AddStoreItemToCart(this);
            // 상점 아이템은 반복 구매 가능하므로 오브젝트를 삭제하지 않음
        }
        else
        {
            PickupToInventory();
        }
    }

    // BuySystem을 찾는 헬퍼 메서드
    private BuySystem GetBuySystem()
    {
        // 1. Inspector에서 할당된 buySystem 우선 사용
        if (buySystem != null)
        {
            return buySystem;
        }

        // 2. 싱글톤 Instance 확인
        if (BuySystem.Instance != null)
        {
            return BuySystem.Instance;
        }

        // 3. 활성화된 BuySystem 찾기
        BuySystem foundBuySystem = FindFirstObjectByType<BuySystem>();
        if (foundBuySystem != null)
        {
            return foundBuySystem;
        }

        // 4. 비활성화된 것도 포함해서 찾기
        foundBuySystem = FindFirstObjectByType<BuySystem>(FindObjectsInactive.Include);
        if (foundBuySystem != null)
        {
            Debug.Log($"비활성화된 BuySystem을 찾았습니다: {foundBuySystem.gameObject.name}");
            return foundBuySystem;
        }

        // 5. 모든 방법으로 찾지 못함
        return null;
    }

    // 아이템 이름을 가져오는 헬퍼 메서드
    private string GetItemName()
    {
        if (storeItemData == null) return "알 수 없는 아이템";
        
        if (storeItemData.dataType == StoreItemDataType.ShopItem)
        {
            return storeItemData.itemName ?? "알 수 없는 상점 아이템";
        }
        else if (storeItemData.dataType == StoreItemDataType.Snack)
        {
            return storeItemData.snackName ?? "알 수 없는 간식";
        }
        
        return "알 수 없는 아이템";
    }

    // 인벤토리에 직접 추가하는 메서드 (기존 기능 유지)
    public void PickupToInventory()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }

        bool success = false;
        string itemName = "";

        if (inventoryManager != null && storeItemData != null)
        {
            if (storeItemData.dataType == StoreItemDataType.ShopItem)
            {
                success = inventoryManager.AddShopItem(storeItemData);
                itemName = storeItemData.itemName;
            }
            else if (storeItemData.dataType == StoreItemDataType.Snack)
            {
                success = inventoryManager.AddSnack(storeItemData);
                itemName = storeItemData.snackName;
            }
        }

        if (success)
        {
            DeleteItem(); // 성공적으로 추가되면 오브젝트 삭제
        }
        else
        {
            Debug.LogWarning($"인벤토리가 가득 차서 {itemName}을(를) 추가할 수 없습니다!");
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager를 찾을 수 없습니다!");
        }
    }

    public void DeleteItem()
    {
        Destroy(gameObject);
    }

    // 하위 호환성을 위한 메서드들
    public void GotShopItem()
    {
        itemType = StoreItemType.ShopItem;
        GotItem();
    }

    public void GotSnackItem()
    {
        itemType = StoreItemType.Snack;
        GotItem();
    }

    public void DeleteShopItem()
    {
        DeleteItem();
    }

    public void DeleteSnackItem()
    {
        DeleteItem();
    }
}
