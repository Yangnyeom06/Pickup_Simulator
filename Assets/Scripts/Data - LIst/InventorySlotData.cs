using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotData : MonoBehaviour
{
    [SerializeField] public Image inventoryimageBG;  // 인벤토리 슬롯 배경 이미지
    [SerializeField] public Image inventoryimage;    // 실제 아이템 이미지
    [SerializeField] private Button ItemSlotButton;
    [SerializeField] public TMP_Text countText;

    public ItemData currentItem;
    public ShopItemData currentShopItem;
    public SnackData currentSnack;
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
        
        // 초기 상태: 아이템 이미지 숨기기
        if (inventoryimage != null)
        {
            inventoryimage.enabled = false;
            inventoryimage.sprite = null;
        }
        
        if (ItemSlotButton != null)
        {
            ItemSlotButton.onClick.RemoveAllListeners();
            ItemSlotButton.onClick.AddListener(OnSlotButtonClicked);
        }
    }

    public void SetItem(ItemData itemData)
    {
        currentItem = itemData;
        currentItemCount = 1;
        
        if (itemData != null && inventoryimage != null)
        {
            // 아이템 아이콘이 null인지 확인
            if (itemData.icon == null)
            {
                Debug.LogError($"아이템 '{itemData.itemName}'의 아이콘이 null입니다! ItemID: {itemData.itemID}");
            }
            
            // 아이템이 있으면 이미지 표시
            inventoryimage.sprite = itemData.icon;
            inventoryimage.enabled = true;
            
            Debug.Log($"아이템 설정 완료: {itemData.itemName} - 아이콘: {(itemData.icon != null ? "있음" : "없음")}");

            if (countText != null) {
                countText.text = currentItemCount.ToString();
            }
        }
        else
        {
            // 아이템이 없으면 이미지 숨기기
            if (inventoryimage != null)
            {
                inventoryimage.sprite = null;
                inventoryimage.enabled = false;
            }
            
            if (countText != null)
            {
                countText.text = "";
            }
            
            if (itemData == null)
            {
                Debug.Log("아이템 데이터가 null이므로 슬롯을 비웁니다.");
            }
            else if (inventoryimage == null)
            {
                Debug.LogError("inventoryimage가 null입니다! 슬롯 프리팹에서 Image 컴포넌트를 확인하세요.");
            }
        }
    }

    public void SetSnack(SnackData snackData)
    {
        currentSnack = snackData;
        currentItemCount = 1;
        
        // 구매일과 유통기한 설정 (현재 날짜로 설정)
        if (snackData != null)
        {
            var dayManager = DayManager.Instance;
            if (dayManager != null)
            {
                snackPurchaseDay = dayManager.CalculateTotalDays();
                snackShelfLifeDays = snackData.shelfLifeDays;
                
                Debug.Log($"간식 설정: {snackData.snackName}, 구매일: {snackPurchaseDay}, 유통기한: {snackShelfLifeDays}일");
            }
            else
            {
                // DayManager가 없으면 기본값 사용
                snackPurchaseDay = -1;
                snackShelfLifeDays = 5;
                Debug.LogWarning("DayManager를 찾을 수 없어서 간식 구매일을 설정할 수 없습니다.");
            }
        }
        else
        {
            snackPurchaseDay = -1;
            snackShelfLifeDays = 5;
        }
        
        if (snackData != null && inventoryimage != null)
        {
            // 스낵 아이콘이 null인지 확인
            if (snackData.icon == null)
            {
                Debug.LogError($"스낵 '{snackData.snackName}'의 아이콘이 null입니다! ItemID: {snackData.itemID}");
            }
            
            // 스낵이 있으면 이미지 표시
            inventoryimage.sprite = snackData.icon;
            inventoryimage.enabled = true;
            
            Debug.Log($"스낵 설정 완료: {snackData.snackName} - 아이콘: {(snackData.icon != null ? "있음" : "없음")}");
            
            if (countText != null)
            {
                countText.text = currentItemCount.ToString();
            }
        }
        else
        {
            // 스낵이 없으면 이미지 숨기기
            if (inventoryimage != null)
            {
                inventoryimage.sprite = null;
                inventoryimage.enabled = false;
            }
            
            if (countText != null)
            {
                countText.text = "";
            }
            
            if (snackData == null)
            {
                Debug.Log("스낵 데이터가 null이므로 슬롯을 비웁니다.");
            }
            else if (inventoryimage == null)
            {
                Debug.LogError("inventoryimage가 null입니다! 슬롯 프리팹에서 Image 컴포넌트를 확인하세요.");
            }
        }
    }

    public void SetShopItem(ShopItemData shopItemData)
    {
        currentShopItem = shopItemData;
        currentItemCount = 1;

        if (shopItemData != null && inventoryimage != null)
        {
            // 상점 아이템 아이콘이 null인지 확인
            if (shopItemData.icon == null)
            {
                Debug.LogError($"상점 아이템 '{shopItemData.itemName}'의 아이콘이 null입니다! ItemID: {shopItemData.itemID}");
            }
            
            // 상점 아이템이 있으면 이미지 표시
            inventoryimage.sprite = shopItemData.icon;
            inventoryimage.enabled = true;
            
            Debug.Log($"상점 아이템 설정 완료: {shopItemData.itemName} - 아이콘: {(shopItemData.icon != null ? "있음" : "없음")}");
            
            if (countText != null)
            {
                countText.text = currentItemCount.ToString();
            }
        }
        else
        {
            // 상점 아이템이 없으면 이미지 숨기기
            if (inventoryimage != null)
            {
                inventoryimage.sprite = null;
                inventoryimage.enabled = false;
            }
            
            if (countText != null)
            {
                countText.text = "";
            }
            
            if (shopItemData == null)
            {
                Debug.Log("상점 아이템 데이터가 null이므로 슬롯을 비웁니다.");
            }
            else if (inventoryimage == null)
            {
                Debug.LogError("inventoryimage가 null입니다! 슬롯 프리팹에서 Image 컴포넌트를 확인하세요.");
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentShopItem = null;
        currentSnack = null;
        currentItemCount = 0;
        
        // 간식 유통기한 정보 초기화
        snackPurchaseDay = -1;
        snackShelfLifeDays = 5;
        
        // 아이템이 없으므로 이미지 숨기기
        if (inventoryimage != null)
        {
            inventoryimage.sprite = null;
            inventoryimage.enabled = false;
        }
        
        if (countText != null)
        {
            countText.text = "";
        }
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

    public void OnInfoButtonClicked()
    {
        if (currentItem != null)
        {
            // 더러움 상태 및 가격 정보 계산
            string dirtyStateInfo = $"더러움 상태: {currentItem.GetDirtyStateString()} ({currentItem.dirty:F2})";
            int penalty = currentItem.GetDirtyPenalty();
            int adjustedPrice = currentItem.GetAdjustedSalePrice();
            string priceInfo = penalty > 0 ? 
                $"기본 가격: {currentItem.price}G → 판매가: {adjustedPrice}G (-{penalty}G)" :
                $"가격/판매가: {currentItem.price}G (페널티 없음)";
            
            Debug.Log($"=== 아이템 정보 ===\n" +
                      $"이름: {currentItem.itemName}\n" +
                      $"희귀도: {currentItem.itemRarity}\n" +
                      $"설명: {currentItem.description}\n" +
                      $"{dirtyStateInfo}\n" +
                      $"{priceInfo}");
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
            // 유통기한 정보 계산
            string expirationInfo = "유통기한 정보 없음";
            var dayManager = DayManager.Instance;
            
            if (dayManager != null && snackPurchaseDay >= 0)
            {
                int currentDay = dayManager.CalculateTotalDays();
                int daysElapsed = currentDay - snackPurchaseDay;
                int remainingDays = snackShelfLifeDays - daysElapsed;
                
                if (remainingDays <= 0)
                {
                    expirationInfo = $"⚠️ 유통기한 만료 (구매 후 {daysElapsed}일 경과)";
                }
                else if (remainingDays == 1)
                {
                    expirationInfo = $"⚠️ 유통기한 1일 남음 (구매일: {snackPurchaseDay}일차)";
                }
                else
                {
                    expirationInfo = $"유통기한 {remainingDays}일 남음 (구매일: {snackPurchaseDay}일차)";
                }
            }
            
            // 효과 타입에 따른 설명
            string effectDescription;
            switch (currentSnack.effectType)
            {
                case SnackEffectType.Health:
                    effectDescription = $"체력 증가: {currentSnack.itemStat}";
                    break;
                case SnackEffectType.Stamina:
                    effectDescription = $"스태미나 증가: {currentSnack.itemStat}";
                    break;
                case SnackEffectType.Both:
                    effectDescription = $"체력 & 스태미나 증가: {currentSnack.itemStat}";
                    break;
                default:
                    effectDescription = $"알 수 없는 효과: {currentSnack.itemStat}";
                    break;
            }
            
            Debug.Log($"=== 스낵 아이템 정보 ===\n" +
                      $"이름: {currentSnack.snackName}\n" +
                      $"설명: {currentSnack.description}\n" +
                      $"가격: {currentSnack.price}G\n" +
                      $"효과: {effectDescription}\n" +
                      $"{expirationInfo}");
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
        inventoryimage.sprite = item.icon;
        inventoryimage.enabled = true;
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
        
        // 유통기한 체크
        var dayManager = DayManager.Instance;
        if (dayManager != null && snackPurchaseDay >= 0)
        {
            int currentDay = dayManager.CalculateTotalDays();
            int daysElapsed = currentDay - snackPurchaseDay;
            int remainingDays = snackShelfLifeDays - daysElapsed;
            
            if (daysElapsed >= snackShelfLifeDays)
            {
                Debug.LogWarning($"⚠️ {currentSnack.snackName}이(가) 유통기한이 지났습니다!");
                Debug.LogWarning($"   구매일: {snackPurchaseDay}일차, 현재: {currentDay}일차");
                Debug.LogWarning($"   경과: {daysElapsed}일, 유통기한: {snackShelfLifeDays}일");
                Debug.LogWarning($"   이 간식은 사용할 수 없습니다.");
                return false;
            }
            else if (remainingDays <= 1)
            {
                Debug.Log($"⚠️ {currentSnack.snackName}의 유통기한이 {remainingDays}일 남았습니다!");
            }
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
            switch (currentSnack.effectType)
            {
                case SnackEffectType.Stamina:
                    if (playerManager.IsStaminaFull())
                    {
                        Debug.Log($"{currentSnack.snackName}: 스태미나가 이미 최대치입니다!");
                        return false;
                    }
                    actualIncrease = playerManager.RestoreStamina(currentSnack.itemStat);
                    effectName = "스태미나";
                    break;
                    
                case SnackEffectType.Health:
                    if (playerManager.IsHealthFull())
                    {
                        Debug.Log($"{currentSnack.snackName}: 체력이 이미 최대치입니다!");
                        return false;
                    }
                    actualIncrease = playerManager.RestoreHealth(currentSnack.itemStat);
                    effectName = "체력";
                    break;
                    
                case SnackEffectType.Both:
                    // 체력과 스태미나 모두 최대치인지 확인
                    if (playerManager.IsHealthFull() && playerManager.IsStaminaFull())
                    {
                        Debug.Log($"{currentSnack.snackName}: 체력과 스태미나가 모두 최대치입니다!");
                        return false;
                    }
                    
                    // 체력과 스태미나 동시 회복
                    float healthIncrease = playerManager.RestoreHealth(currentSnack.itemStat);
                    float staminaIncrease = playerManager.RestoreStamina(currentSnack.itemStat);
                    actualIncrease = healthIncrease + staminaIncrease; // 총 회복량
                    effectName = $"체력 +{healthIncrease}, 스태미나 +{staminaIncrease}";
                    break;
                    
                default:
                    Debug.LogWarning($"{currentSnack.snackName}: 알 수 없는 효과 타입 {currentSnack.effectType}");
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
            Debug.Log($"{currentSnack.snackName} 사용! {effectName} +{actualIncrease}");
            
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
            Debug.Log($"{currentSnack.snackName}: {effectName} 증가 실패");
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
