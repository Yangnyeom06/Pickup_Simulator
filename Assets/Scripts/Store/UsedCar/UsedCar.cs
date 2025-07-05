using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UsedCar : MonoBehaviour, IDropHandler
{
    public PlayerData playerData;
    public MoneyManager moneyManager;

    public void OnDrop(PointerEventData eventData)
    {
        var itemUI = eventData.pointerDrag.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            // 안전하게 캐스팅
            ShopItemData shopItem = itemUI.shopItemData;
            if (shopItem != null)
            {
                SellItem(shopItem);
            }
            else
            {
                Debug.LogWarning("판매할 수 없는 아이템입니다.");
            }
        }
    }

    private void SellItem(ShopItemData item)
    {
        // 판매 금액 지급
        playerData.money += item.price;
        moneyManager.UpdateMoneyUI();

        // 인벤토리에서 아이템 제거
        InventoryManager.Instance.RemoveItemById(item.itemCode);

        Debug.Log($"{item.itemName}을(를) 판매했습니다!");
    }
}
