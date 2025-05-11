using UnityEngine;
using UnityEngine.UI;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] private Image ItemSlotImage;
    [SerializeField] private Button ItemSlotButton;
    public ItemData currentItem;

    private void Awake()
    {
        ItemSlotButton.onClick.AddListener(OnInfoButtonClicked);
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;
        if (item != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = item.icon;
            ItemSlotImage.enabled = true;
        }
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
        else
        {
            Debug.Log("아이템이 없습니다.");
        }
    }
}
