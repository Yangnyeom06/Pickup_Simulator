using UnityEngine;
using UnityEngine.EventSystems;

public class AntiqueShop : MonoBehaviour, IDropHandler
{
    public PlayerData playerData;
    public MoneyManager moneyManager;

    public void OnDrop(PointerEventData eventData)
    {
        // 드롭된 오브젝트에서 아이템 정보 추출
        var itemUI = eventData.pointerDrag.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            ShopItemData shopItem = itemUI.shopItemData;
            SellItem(shopItem);
        }
    }

    private void SellItem(ShopItemData shopItem)
    {
        // 판매 금액 지급
        playerData.money += shopItem.price;
        moneyManager.UpdateMoneyUI();

        // 인벤토리에서 아이템 제거
        InventoryManager.Instance.RemoveItemById(shopItem.itemCode);

        // (선택) 판매 완료 메시지
        Debug.Log($"{shopItem.itemName}을(를) 판매했습니다!");
    }
}
