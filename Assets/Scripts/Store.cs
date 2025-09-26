using Unity.VisualScripting;
using UnityEngine;


public class ShopItem : MonoBehaviour
{
    public ShopItemData shopItemData; // 아이템 데이터 참조
    InventoryManager inventoryManager;
/*
    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null)
        {
            if (inventoryManager.AddItem(itemData)) // 성공한 경우만 삭제
            {
                DeleteItem();
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
*/
    public void SetShopItemData(ShopItemData data)
    {
        shopItemData = data;
    }

    public void GotShopItem()
    {
        bool success = inventoryManager.AddShopItem(shopItemData);
        if (!success)
        {
            Debug.LogWarning($"인벤토리가 가득 차서 {shopItemData.itemName}을(를) 추가할 수 없습니다!");
        }
    }

    // ItemRaycast와 호환을 위한 PickupItem 메서드
    public void PickupItem()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }

        if (inventoryManager != null && shopItemData != null)
        {
            bool success = inventoryManager.AddShopItem(shopItemData);
            if (success)
            {
                Debug.Log($"{shopItemData.itemName}을(를) 인벤토리에 추가했습니다!");
                DeleteShopItem(); // 성공적으로 추가되면 오브젝트 삭제
            }
            else
            {
                Debug.LogWarning($"인벤토리가 가득 차서 {shopItemData.itemName}을(를) 추가할 수 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager 또는 ShopItemData를 찾을 수 없습니다!");
        }
    }

    public void DeleteShopItem()
    {
        Destroy(gameObject);
    }
}