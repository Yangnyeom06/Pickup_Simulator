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
    private PlayerManager playerManager;

    public InventorySlotData originalInventorySlot;
    // private JunkyardNPC junkyardNPC;


    private void Awake()
    {
        saleSystem = Object.FindFirstObjectByType<SaleSystem>();
        playerManager = Object.FindFirstObjectByType<PlayerManager>();
        
        if (playerManager == null)
        {
            Debug.LogError("InventorySlotData: PlayerManager를 찾을 수 없습니다! 씬에 PlayerManager가 있는지 확인하세요.");
        }
        
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
        currentItemCount = 1;
        
        if (snackData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = snackData.icon;
            ItemSlotImage.enabled = true;
            
            if (countText != null)
            {
                countText.text = currentItemCount.ToString();
            }
        }
        else if(ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
            
            if (countText != null)
            {
                countText.text = "";
            }
        }
    }

    public void SetShopItem(ShopItemData shopItemData)
    {
        currentShopItem = shopItemData;
        currentItemCount = 1;

        if (shopItemData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = shopItemData.icon;
            ItemSlotImage.enabled = true;
            
            if (countText != null)
            {
                countText.text = currentItemCount.ToString();
            }
        }
        else if (ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
            
            if (countText != null)
            {
                countText.text = "";
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentShopItem = null;
        currentSnack = null;
        currentItemCount = 0;
        ItemSlotImage.sprite = null;
        ItemSlotImage.enabled = false;
        
        if (countText != null)
        {
            countText.text = "";
        }
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
        else if (currentSnack != null)
        {
            Debug.Log($"[스낵 아이템]\n" +
                      $"이름: {currentSnack.snackName}\n" +
                      $"설명: {currentSnack.description}\n" +
                      $"가격: {currentSnack.price}\n" +
                      $"스탯 증가: {currentSnack.itemStat}");
        }
        else
        {
            Debug.Log("아이템이 없습니다.");
        }
    }

    // //store 시스템을 위한 RemoveItem 함수 추가 -  수민
    // public void RemoveItem(int count)
    // {
    //     if (currentItem == null)
    //     {
    //         Debug.LogWarning("아이템이 없습니다");
    //         return;
    //     }
        
    //     // // 실제로 제거할 수 있는 수량만큼만 제거
    //     // int removeCount = Mathf.Min(count, currentItemCount);
    //     // currentItemCount -= removeCount;

    //     // if (currentItemCount <= 0)
    //     // {
    //     //     currentItem = null;
    //     //     currentItemCount = 0;
    //     //     ItemSlotImage.enabled = false;
    //     // }
        
    //     // if (countText != null)
    //     // countText.text = currentItemCount.ToString();
    // }

    public void SetupSlot(ItemData item, int count, ISaleSystem system)
    {
        saleSystem = system;
        currentItem        = item;
        // currentItemCount   = count;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => saleSystem.OnSlotClicked(this));


        // 아이콘 & 수량 UI 갱신
        ItemSlotImage.sprite = item.icon;
        ItemSlotImage.enabled = true;
        // if (countText != null)
        //     countText.text = currentItemCount.ToString();

        // 클릭 리스너: 이 슬롯이 클릭되면 바로 SaleSystem.OnSlotClicked(this)
        ItemSlotButton.onClick.RemoveAllListeners();
        ItemSlotButton.onClick.AddListener(OnSlotButtonClicked);
    }
    
    public void OnSlotButtonClicked()
    {
        // 어떤 아이템이 있는지 확인하고 로그 출력
        string itemName = "빈 슬롯";
        
        if (currentItem != null)
        {
            itemName = currentItem.itemName;
        }
        else if (currentShopItem != null)
        {
            itemName = currentShopItem.itemName;
        }
        else if (currentSnack != null)
        {
            itemName = currentSnack.snackName;
            
            // 판매 시스템이 활성화된 경우 판매 모드, 그렇지 않으면 사용 모드
            if (IsSellModeActive())
            {
                Debug.Log($"[판매 모드] {itemName} 판매 준비");
                // 판매 로직은 아래에서 처리
            }
            else
            {
                // 스낵 아이템 사용 처리
                if (TryUseSnack())
                {
                    return; // 스낵을 사용했으면 여기서 끝
                }
            }
        }
        
        Debug.Log($"[InventorySlotData] 슬롯 클릭: {itemName}");
        
        // SellUI가 활성화된 상태라면 판매 모드로 간주
        saleSystem?.OnSlotClicked(this);

        // 그렇지 않으면 기존 정보 출력
        OnInfoButtonClicked();
    }

    /// <summary>
    /// 스낵 아이템을 사용하여 스태미나를 증가시킵니다
    /// </summary>
    /// <returns>스낵을 성공적으로 사용했으면 true</returns>
    private bool TryUseSnack()
    {
        if (currentSnack == null)
        {
            Debug.LogWarning("스낵 데이터가 없습니다!");
            return false;
        }

        // playerManager가 null이면 다시 찾아보기
        if (playerManager == null)
        {
            playerManager = Object.FindFirstObjectByType<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogError("PlayerManager를 찾을 수 없습니다! 씬에 PlayerManager가 있는지 확인하세요.");
                return false;
            }
        }

        // 스태미나가 이미 최대치인지 확인
        float actualIncrease = 0f;
        
        try
        {
            if (playerManager.IsStaminaFull())
            {
                Debug.Log($"{currentSnack.snackName}: 스태미나가 이미 최대치입니다!");
                return false;
            }

            // 스태미나 증가
            float staminaIncrease = currentSnack.itemStat;
            actualIncrease = playerManager.RestoreStamina(staminaIncrease);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스태미나 처리 중 오류 발생: {e.Message}");
            return false;
        }

        if (actualIncrease > 0)
        {
            Debug.Log($"{currentSnack.snackName} 사용! 스태미나 +{actualIncrease}");
            
            // 아이템 수량 감소
            currentItemCount--;
            
            if (currentItemCount <= 0)
            {
                // 아이템을 모두 사용했으면 슬롯 비우기
                string snackName = currentSnack.snackName; // ClearSlot 전에 이름 저장
                ClearSlot();
                Debug.Log($"{snackName}을(를) 모두 사용했습니다."); // snackName 변수 사용
            }
            else
            {
                // 수량 텍스트 업데이트
                if (countText != null)
                {
                    countText.text = currentItemCount.ToString();
                }
            }
            
            return true; // 스낵 사용 성공
        }
        else
        {
            Debug.Log($"{currentSnack.snackName}: 스태미나 증가 실패");
            return false;
        }
    }

    /// <summary>
    /// 현재 판매 모드가 활성화되어 있는지 확인
    /// </summary>
    /// <returns>판매 모드가 활성화되어 있으면 true</returns>
    private bool IsSellModeActive()
    {
        // saleSystem이 null이거나 비활성화된 경우 false
        if (saleSystem == null) return false;
        
        // SaleSystem이 MonoBehaviour를 상속받는다면 GameObject 활성화 상태 확인
        if (saleSystem is MonoBehaviour saleSystemMB)
        {
            return saleSystemMB.gameObject.activeInHierarchy;
        }
        
        // 그렇지 않으면 saleSystem이 존재하면 활성화된 것으로 간주하지 않음 (사용 모드 우선)
        return false;
    }

}
