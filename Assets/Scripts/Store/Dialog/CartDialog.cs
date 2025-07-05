using UnityEngine;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

public class CartItemSlot : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text quantityText;
    public Button plusButton;
    public Button minusButton;

    private ShopItemData itemData;
    private BuySystem buySystem;

    public void Setup(ShopItemData data, int quantity, BuySystem system)
    {
        itemData = data;
        buySystem = system;
        itemNameText.text = data.itemName;
        quantityText.text = quantity.ToString();

        plusButton.onClick.AddListener(() => buySystem.AdjustItemQuantity(itemData, 1));
        minusButton.onClick.AddListener(() => buySystem.AdjustItemQuantity(itemData, -1));
    }

    public void UpdateQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
    }
}
