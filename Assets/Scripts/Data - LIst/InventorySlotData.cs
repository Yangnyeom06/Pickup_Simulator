using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] public Image ItemSlotImage;
    [SerializeField] private Button ItemSlotButton;
    [SerializeField] public TMP_Text countText;

    public ItemData currentItem;
    public StoreItemData currentShopItem;
    public StoreItemData currentSnackItem;
    public int currentItemCount = 1; // 슬롯에 들어있는 아이템 수량


    [Header("유통기한 관련 (간식용)")]
    public int snackPurchaseDay = -1; // 간식 구매일 (-1은 미설정)
    public int snackShelfLifeDays = 5; // 간식 유통기한


    [System.NonSerialized]
    private ISaleSystem saleSystem;
    [System.NonSerialized]
    private PlayerManager playerManager;

    [System.NonSerialized]
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

            if (countText != null)
            {
                countText.text = currentItemCount.ToString();
            }
        }
        else if (ItemSlotImage != null)
        {
            ItemSlotImage.sprite = null;
            ItemSlotImage.enabled = false;
        }
    }

    public void SetSnackItem(StoreItemData storeItemData)
    {
        SetStoreItem(storeItemData, StoreItemDataType.Snack);
    }

    public void SetShopItem(StoreItemData storeItemData)
    {
        SetStoreItem(storeItemData, StoreItemDataType.ShopItem);
    }

    // 통합된 StoreItem 설정 메서드
    public void SetStoreItem(StoreItemData storeItemData, StoreItemDataType? expectedType = null)
    {
        // 타입 검증 (선택적)
        if (expectedType.HasValue && storeItemData != null && storeItemData.dataType != expectedType.Value)
        {
            Debug.LogWarning($"예상된 타입 {expectedType}과 실제 타입 {storeItemData.dataType}이 다릅니다!");
        }

        // 타입에 따라 적절한 필드에 할당
        if (storeItemData != null)
        {
            if (storeItemData.dataType == StoreItemDataType.ShopItem)
            {
                currentShopItem = storeItemData;
                currentSnackItem = null;
            }
            else if (storeItemData.dataType == StoreItemDataType.Snack)
            {
                currentSnackItem = storeItemData;
                currentShopItem = null;
            }
        }
        else
        {
            currentShopItem = null;
            currentSnackItem = null;
        }

        currentItemCount = 1;

        if (storeItemData != null && ItemSlotImage != null)
        {
            ItemSlotImage.sprite = storeItemData.icon;
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
        currentSnackItem = null;
        currentItemCount = 0;
        ItemSlotImage.sprite = null;
        ItemSlotImage.enabled = false;
        
        if (countText != null)
        {
            countText.text = "";
        }
    }

    /// <summary>
    /// 같은 아이템이 있는 슬롯에 아이템 개수를 증가시킵니다
    /// </summary>
    /// <param name="amount">증가시킬 개수</param>
    /// <returns>실제로 추가된 개수</returns>
    public int AddItemCount(int amount = 1)
    {
        if (currentItem == null) return 0;
        
        int maxStack = currentItem.maxStackSize;
        int canAdd = maxStack - currentItemCount;
        int actualAdded = Mathf.Min(amount, canAdd);
        
        currentItemCount += actualAdded;
        
        // UI 업데이트
        if (countText != null)
        {
            countText.text = currentItemCount.ToString();
        }
        
        return actualAdded;
    }

    /// <summary>
    /// 같은 아이템인지 확인합니다 (itemID 기준)
    /// </summary>
    /// <param name="otherItem">비교할 아이템</param>
    /// <returns>같은 아이템이면 true</returns>
    public bool IsSameItem(ItemData otherItem)
    {
        if (currentItem == null || otherItem == null)
        {
            Debug.Log($"[IsSameItem] null 체크 실패 - currentItem: {currentItem != null}, otherItem: {otherItem != null}");
            return false;
        }
        
        // 다양한 방법으로 아이템 비교
        bool isSame = false;
        
        // 1순위: ScriptableObject 참조 직접 비교 (가장 정확)
        if (currentItem == otherItem)
        {
            isSame = true;
            Debug.Log($"[IsSameItem] 참조 비교: 같은 ScriptableObject → {isSame}");
        }
        // 2순위: itemID 비교
        else if (!string.IsNullOrEmpty(currentItem.itemID) && !string.IsNullOrEmpty(otherItem.itemID))
        {
            isSame = currentItem.itemID == otherItem.itemID;
            Debug.Log($"[IsSameItem] ID 비교: '{currentItem.itemName}' (ID: '{currentItem.itemID}') vs '{otherItem.itemName}' (ID: '{otherItem.itemID}') → {isSame}");
        }
        // 3순위: itemName 비교
        else
        {
            isSame = currentItem.itemName == otherItem.itemName;
            Debug.Log($"[IsSameItem] 이름 비교: '{currentItem.itemName}' vs '{otherItem.itemName}' → {isSame}");
        }
        
        return isSame;
    }

    /// <summary>
    /// 이 슬롯에 더 많은 아이템을 추가할 수 있는지 확인합니다
    /// </summary>
    /// <returns>추가 가능하면 true</returns>
    public bool CanAddMore()
    {
        if (currentItem == null) return false;
        return currentItemCount < currentItem.maxStackSize;
    }


    // 디버그용
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
        else if (currentSnackItem != null)
        {
            Debug.Log($"[스낵 아이템]\n" +
                      $"이름: {currentSnackItem.snackName}\n" +
                      $"설명: {currentSnackItem.description}\n" +
                      $"가격: {currentSnackItem.price}\n" +
                      $"스탯 증가: {currentSnackItem.itemStat}");
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
        Debug.Log($"[SetupSlot] ★★★ 슬롯 설정 시작 ★★★");
        Debug.Log($"[SetupSlot] item: {item}");
        Debug.Log($"[SetupSlot] item.itemName: {(item != null ? item.itemName : "NULL")}");
        Debug.Log($"[SetupSlot] count: {count}");
        Debug.Log($"[SetupSlot] system: {system}");
        
        // Null 체크 추가
        if (item == null)
        {
            Debug.LogError("[SetupSlot] item이 null입니다! 슬롯 설정을 중단합니다.");
            return;
        }
        
        saleSystem = system;
        currentItem = item;
        currentItemCount = count;
        
        Debug.Log($"[SetupSlot] 설정 완료 - currentItem: {currentItem}");

        // Main Button 설정 (판매 시스템용)
        var mainButton = GetComponent<Button>();
        if (mainButton != null)
        {
            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(() => {
                Debug.Log($"[SetupSlot] ★★★ 메인 버튼 클릭됨: {item.itemName} ★★★");
                Debug.Log($"[SetupSlot] 클릭 시 currentItem: {currentItem}");
                Debug.Log($"[SetupSlot] 클릭 시 saleSystem: {saleSystem}");
                saleSystem.OnSlotClicked(this);
            });
        }
        else
        {
            Debug.LogError($"[SetupSlot] GetComponent<Button>()가 null을 반환했습니다! GameObject: {gameObject.name}");
        }

        // 아이콘 & 수량 UI 갱신
        if (ItemSlotImage != null)
        {
            ItemSlotImage.sprite = item.icon;
            ItemSlotImage.enabled = true;
        }
        else
        {
            Debug.LogWarning("[SetupSlot] ItemSlotImage가 null입니다!");
        }
        
        if (countText != null)
        {
            countText.text = currentItemCount.ToString();
        }

        // ItemSlotButton도 판매 시스템에 연결 (보조 버튼)
        if (ItemSlotButton != null)
        {
            ItemSlotButton.onClick.RemoveAllListeners();
            ItemSlotButton.onClick.AddListener(() => {
                Debug.Log($"[SetupSlot] ★★★ ItemSlotButton 클릭됨: {item.itemName} ★★★");
                Debug.Log($"[SetupSlot] 클릭 시 currentItem: {currentItem}");
                Debug.Log($"[SetupSlot] 클릭 시 saleSystem: {saleSystem}");
                saleSystem.OnSlotClicked(this);
            });
            ItemSlotButton.interactable = true;
            Debug.Log($"[SetupSlot] ItemSlotButton 이벤트 설정 완료: {item.itemName}");
        }
        else
        {
            Debug.LogWarning($"[SetupSlot] ItemSlotButton이 null입니다! GameObject: {gameObject.name}");
        }
        
        Debug.Log($"[SetupSlot] 슬롯 설정 완료: {item.itemName}");
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
        else if (currentSnackItem != null)
        {
            itemName = currentSnackItem.snackName;

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
        
    if (currentItem != null && !string.IsNullOrEmpty(currentItem.itemID))
    {
        if (currentItem.itemID == InventoryManager.Instance?.wetWipeItemId)
        {
            InventoryManager.Instance.TryEnterCleanMode(this);
            InventoryManager.Instance.inventory[0].SetActive(true);
            InventoryManager.Instance.inventory[1].SetActive(false);

            return; // 물티슈 클릭 시 여기서 종료 (안전)
        }
    }

    // 2) 청소 모드가 활성화되어 있고, 이 슬롯이 타겟이면 적용
    if (InventoryManager.Instance != null && InventoryManager.Instance.isCleanMode)
    {
        InventoryManager.Instance.ApplyWetWipeTo(this);
        return; // 청소 완료 후 종료
    }


    }

    /// <summary>
    /// 스낵 아이템을 사용하여 스태미나를 증가시킵니다
    /// </summary>
    /// <returns>스낵을 성공적으로 사용했으면 true</returns>
    private bool TryUseSnack()
    {
        if (currentSnackItem == null)
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

        // 간식 효과 타입에 따른 처리
        float actualIncrease = 0f;
        string effectName = "";
        
        try
        {
            switch (currentSnackItem.effectType)
            {
                case SnackEffectType.Stamina:
                    if (playerManager.IsStaminaFull())
                    {
                        Debug.Log($"{currentSnackItem.snackName}: 스태미나가 이미 최대치입니다!");
                        return false;
                    }
                    actualIncrease = playerManager.RestoreStamina(currentSnackItem.itemStat);
                    effectName = "스태미나";
                    break;
                    
                case SnackEffectType.Health:
                    if (playerManager.IsHealthFull())
                    {
                        Debug.Log($"{currentSnackItem.snackName}: 체력이 이미 최대치입니다!");
                        return false;
                    }
                    actualIncrease = playerManager.RestoreHealth(currentSnackItem.itemStat);
                    effectName = "체력";
                    break;
                    
                case SnackEffectType.Both:
                    // 체력과 스태미나 모두 최대치인지 확인
                    if (playerManager.IsHealthFull() && playerManager.IsStaminaFull())
                    {
                        Debug.Log($"{currentSnackItem.snackName}: 체력과 스태미나가 모두 최대치입니다!");
                        return false;
                    }
                    
                    // 체력과 스태미나 동시 회복
                    float healthIncrease = playerManager.RestoreHealth(currentSnackItem.itemStat);
                    float staminaIncrease = playerManager.RestoreStamina(currentSnackItem.itemStat);
                    actualIncrease = healthIncrease + staminaIncrease; // 총 회복량
                    effectName = $"체력 +{healthIncrease}, 스태미나 +{staminaIncrease}";
                    break;
                    
                case SnackEffectType.HealthLossReduction:
                    // 체력 소모량 감소 버프 적용
                    playerManager.ApplyBuff(
                        SnackEffectType.HealthLossReduction,
                        currentSnackItem.itemStat,
                        currentSnackItem.buffDuration,
                        currentSnackItem.snackName
                    );
                    effectName = $"체력 소모량 {currentSnackItem.itemStat}% 감소 ({currentSnackItem.buffDuration}초)";
                    actualIncrease = 1f; // 버프 적용 성공 표시
                    break;
                    
                case SnackEffectType.StaminaLossReduction:
                    // 스테미나 소모량 감소 버프 적용
                    playerManager.ApplyBuff(
                        SnackEffectType.StaminaLossReduction,
                        currentSnackItem.itemStat,
                        currentSnackItem.buffDuration,
                        currentSnackItem.snackName
                    );
                    effectName = $"스테미나 소모량 {currentSnackItem.itemStat}% 감소 ({currentSnackItem.buffDuration}초)";
                    actualIncrease = 1f; // 버프 적용 성공 표시
                    break;
                    
                case SnackEffectType.RunSpeedBoost:
                    // 달리기 속도 증가 버프 적용
                    playerManager.ApplyBuff(
                        SnackEffectType.RunSpeedBoost,
                        currentSnackItem.itemStat,
                        currentSnackItem.buffDuration,
                        currentSnackItem.snackName
                    );
                    effectName = $"달리기 속도 {currentSnackItem.itemStat}% 증가 ({currentSnackItem.buffDuration}초)";
                    actualIncrease = 1f; // 버프 적용 성공 표시
                    break;
                    
                default:
                    Debug.LogWarning($"{currentSnackItem.snackName}: 알 수 없는 효과 타입 {currentSnackItem.effectType}");
                    return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스태미나 처리 중 오류 발생: {e.Message}");
            return false;
        }

        if (actualIncrease > 0)
        {
            // 버프 타입인지 확인
            bool isBuffType = currentSnackItem.effectType == SnackEffectType.HealthLossReduction ||
                              currentSnackItem.effectType == SnackEffectType.StaminaLossReduction ||
                              currentSnackItem.effectType == SnackEffectType.RunSpeedBoost;
            
            if (isBuffType)
            {
                Debug.Log($"{currentSnackItem.snackName} 사용! {effectName}");
            }
            else
            {
                Debug.Log($"{currentSnackItem.snackName} 사용! {effectName}");
            }
            
            // 아이템 수량 감소
            currentItemCount--;
            
            if (currentItemCount <= 0)
            {
                // 아이템을 모두 사용했으면 슬롯 비우기
                string snackName = currentSnackItem.snackName; // ClearSlot 전에 이름 저장
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
            Debug.Log($"{currentSnackItem.snackName}: 사용 실패");
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
