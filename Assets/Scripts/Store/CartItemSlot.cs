using UnityEngine;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

public class CartItemSlot : MonoBehaviour
{
    public TMP_Text itemNameText;
    private ShopItemData itemData;
    private SnackData snackData;
    private BuySystem buySystem;

    public void ItemSetup(ShopItemData itemData, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");

        itemData = itemData;
        snackData = null;
        buySystem = system;

        itemNameText.text = $"{itemData.itemName}";
    }

    public void SnackSetup(SnackData snackData, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");

        snackData = snackData;
        itemData = null;    
        buySystem = system; 

        itemNameText.text = $"{snackData.snackName}";
    }
}
