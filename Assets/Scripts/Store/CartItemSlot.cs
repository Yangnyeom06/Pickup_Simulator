using UnityEngine;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

public class CartItemSlot : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text quantityText;    // 수량 텍스트
    public Button deleteButton;      // 삭제 버튼
    
    private ShopItemData itemData;
    private SnackData snackData;
    private BuySystem buySystem;

    private void Start()
    {
        // 삭제 버튼 클릭 이벤트 연결
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }
    }

    public void ItemSetup(ShopItemData data, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");

        this.itemData = data;
        this.snackData = null;
        this.buySystem = system;

        itemNameText.text = $"{data.itemName}";
        if (quantityText != null)
            quantityText.text = $"x{quantity}";
    }

    public void SnackSetup(SnackData data, int quantity, BuySystem system)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is null");

        this.snackData = data;
        this.itemData = null;    
        this.buySystem = system; 

        itemNameText.text = $"{data.snackName}";
        if (quantityText != null)
            quantityText.text = $"x{quantity}";
    }

    // 삭제 버튼 클릭 시 호출
    public void OnDeleteButtonClicked()
    {
        if (buySystem == null)
        {
            Debug.LogError("BuySystem이 연결되지 않았습니다!");
            return;
        }

        // 스낵 아이템 삭제
        if (snackData != null)
        {
            buySystem.DeleteSnackFromCart(snackData);
        }
        // 일반 아이템 삭제
        else if (itemData != null)
        {
            buySystem.DeleteShopItemFromCart(itemData);
        }
        else
        {
            Debug.LogWarning("삭제할 아이템 데이터가 없습니다!");
        }
    }
}
