using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] public Image ItemSlotImage;
    [SerializeField] private Button ItemSlotButton;
    [SerializeField] public TMP_Text countText;

    public ItemData currentItem;
    public ShopItemData currentShopItem;
    public SnackData currentSnack;
    public int currentItemCount = 1; // 슬롯에 들어있는 아이템 수량

    private ISaleSystem saleSystem;

    public InventorySlotData originalInventorySlot;
    // private JunkyardNPC junkyardNPC;


    private void Awake()
    {
        saleSystem = Object.FindFirstObjectByType<SaleSystem>();
        ItemSlotButton.onClick.RemoveAllListeners();
        ItemSlotButton.onClick.AddListener(OnSlotButtonClicked);
    }

    public void SetItem(ItemData itemData)
    {
        currentItem = itemData;
        currentItemCount = 1;
        if (itemData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = itemData.icon;
            ItemSlotImage.enabled = true;

            if (countText != null) {
                countText.text = currentItemCount.ToString();
            }
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

    public void OnInfoButtonClicked()
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
        
        if (countText != null)
        countText.text = currentItemCount.ToString();
    }

    /// <summary>
    /// SellUI용 슬롯 초기화: 데이터 모델과 버튼 이벤트를 여기서 확실히 셋업!
    /// </summary>
    public void SetupSlot(ItemData item, int count, ISaleSystem system)
    {
        saleSystem = system;
        currentItem        = item;
        currentItemCount   = count;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => saleSystem.OnSlotClicked(this));


        // 아이콘 & 수량 UI 갱신
        ItemSlotImage.sprite = item.icon;
        ItemSlotImage.enabled = true;
        if (countText != null)
            countText.text = currentItemCount.ToString();

        // 클릭 리스너: 이 슬롯이 클릭되면 바로 SaleSystem.OnSlotClicked(this)
        ItemSlotButton.onClick.RemoveAllListeners();
        ItemSlotButton.onClick.AddListener(OnSlotButtonClicked);
    }
    
    public void OnSlotButtonClicked()
    {
        Debug.Log($"[InventorySlotData] 슬롯 클릭: {currentItem.itemName} x{currentItemCount}");
        // SellUI가 활성화된 상태라면 판매 모드로 간주
        saleSystem?.OnSlotClicked(this);

        // 그렇지 않으면 기존 정보 출력
        OnInfoButtonClicked();
    }

}
