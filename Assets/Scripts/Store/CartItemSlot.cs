using UnityEngine;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

public class CartItemSlot : MonoBehaviour
{
    public TMP_Text itemNameText;
    private ShopItemData itemData;
    private SnackData snackData;
    private BuySystem buySystem;

    public void ItemSetup(ShopItemData data, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");

        itemData = data;
        buySystem = system;
        itemNameText.text = data.itemName;
    }

    public void SnackSetup(SnackData data, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");


        snackData = data;
        itemData = null;
        buySystem = system;

    }
}
