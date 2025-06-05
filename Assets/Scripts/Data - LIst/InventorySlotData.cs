using UnityEngine;
using UnityEngine.UI;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] public Image ItemSlotImage;
    [SerializeField] private Button ItemSlotButton;
    public ItemData currentItem;
    public ShopItemData currentShopItem;
    public int currentItemCount = 1; // 슬롯에 들어있는 아이템 수량


    private void Awake()
    {
        ItemSlotButton.onClick.AddListener(OnInfoButtonClicked);
    }

    public void SetItem(ItemData itemData)
    {
        currentItem = itemData;
        if (itemData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = itemData.icon;
            ItemSlotImage.enabled = true;
        }
        else if(ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
        }
    }

    public void SetShopItem(ShopItemData shopItemData)
    {
        currentShopItem = shopItemData;

        if (shopItemData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = shopItemData.icon;
            ItemSlotImage.enabled = true;
        }
        else if (ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentShopItem = null;
        currentItemCount = 0;
        ItemSlotImage.sprite = null;
        ItemSlotImage.enabled = false;
    }

    private void OnInfoButtonClicked()
    {
        if (currentItem != null)
        {
            Debug.Log($"아이템 이름: {currentItem.itemName}\n" +
                      $"희귀도: {currentItem.itemRarity}\n" +
                      $"설명: {currentItem.description}\n" +
                      $"가치: {currentItem.value}");
        }
        else if (currentShopItem != null)
        {
            Debug.Log($"[상점 아이템]\n" +
                      $"이름: {currentShopItem.itemName}\n" +
                      $"설명: {currentShopItem.description}\n" +
                      $"가격: {currentShopItem.price}");
        }
        else
        {
            Debug.Log("아이템이 없습니다.");
        }
    }

    //store 시스템을 위한 RemoveItem 함수 추가 -  수민
    
    public void RemoveItem(int count)
    {
        if (currentItem == null || currentItemCount <= 0)
        {
            Debug.LogWarning("슬롯이 비어있거나 수량이 0 이하입니다.");
            return;
        }
        
        // 실제로 제거할 수 있는 수량만큼만 제거
        int removeCount = Mathf.Min(count, currentItemCount);
        currentItemCount -= removeCount;

        if (currentItemCount <= 0)
        {
            currentItem = null;
            currentItemCount = 0;
            ItemSlotImage.enabled = false;
        }
        // UI 갱신
    }
}
