using UnityEngine;
using UnityEngine.UI;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] private Image ItemSlotImage;
    [SerializeField] private Button ItemSlotButton;
    public ItemData currentItem;
    public SnackData currentSnack;

    public IntStatValueSO steminaLevel;
    public PlayerController playerController;

    private void Awake()
    {
        ItemSlotButton.onClick.AddListener(OnInfoButtonClicked);
        ItemSlotButton.onClick.AddListener(OnSlotClicked);
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

    public void SetSnack(SnackData snackData)
    {
        currentSnack = snackData;
        if (snackData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = snackData.icon;
            ItemSlotImage.enabled = true;
        }
        else if(ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
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
        else if (currentSnack != null)
        {
            Debug.Log($"아이템 이름: {currentSnack.itemName}\n" +
                      $"스탯: {currentSnack.itemStat}\n" +
                      $"설명: {currentSnack.description}\n");
        }
        else
        {
            Debug.Log("아이템이 없습니다.");
        }
    }

    private void OnSlotClicked()
    {
        if (currentSnack != null && steminaLevel != null)
        {
            float newStemina = steminaLevel.current + currentSnack.itemStat;

            // 최대치를 초과하지 않도록 제한
            if (newStemina > steminaLevel.max) newStemina = steminaLevel.max;

            steminaLevel.current = newStemina;

            Debug.Log($"{currentSnack.itemName}을 사용하여 스테미나가 {currentSnack.itemStat} 만큼 증가했습니다. 현재 스테미나: {steminaLevel.current}");

            // 사용한 간식 삭제
            currentSnack = null;
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
        }
    }
}
