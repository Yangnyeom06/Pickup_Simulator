using UnityEngine;
using UnityEngine.UI; // Button을 위한 네임스페이스
using TMPro;           // TMP_Text를 위한 네임스페이스

public class CartItemSlot : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text quantityText;    // 수량 텍스트
    public Button deleteButton;      // 삭제 버튼
    
    private StoreItemData itemData;
    private BuySystem buySystem;
    private bool isProcessing = false; // 중복 호출 방지 플래그

    private void Start()
    {
        SetupDeleteButton();
    }
    
    private void SetupDeleteButton()
    {
        // 삭제 버튼 클릭 이벤트 연결
        if (deleteButton != null)
        {
            // 기존 리스너 제거 후 새로 추가 (중복 방지)
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }
    }

    public void ItemSetup(StoreItemData data, int quantity, BuySystem system)
    {
        this.itemData = data;
        this.buySystem = system;

        // StoreItemData의 타입에 따라 적절한 이름 표시
        if (data.dataType == StoreItemDataType.ShopItem)
        {
            itemNameText.text = $"{data.itemName}";
        }
        else if (data.dataType == StoreItemDataType.Snack)
        {
            itemNameText.text = $"{data.snackName}";
        }
        
        quantityText.text = $"{quantity}개";
            
        // Setup 시에도 버튼 이벤트 재설정
        SetupDeleteButton();
    }

    // 하위 호환성을 위한 메서드들 (기존 코드가 호출할 수 있도록)
    public void SnackSetup(StoreItemData data, int quantity, BuySystem system)
    {
        ItemSetup(data, quantity, system);
    }

    // 삭제 버튼 클릭 시 호출
    public void OnDeleteButtonClicked()
    {
        // 중복 호출 방지
        if (isProcessing)
        {
            return;
        }
        
        isProcessing = true;
        
        if (buySystem == null)
        {
            isProcessing = false;
            return;
        }

        // 아이템 삭제 (StoreItemData 타입에 따라 적절한 메서드 호출)
        if (itemData != null)
        {
            if (itemData.dataType == StoreItemDataType.Snack)
            {
                buySystem.DeleteSnackFromCart(itemData);
            }
            else if (itemData.dataType == StoreItemDataType.ShopItem)
            {
                buySystem.DeleteShopItemFromCart(itemData);
            }
        }
        
        // 처리 완료 후 플래그 해제 (약간의 지연을 두어 중복 클릭 방지)
        StartCoroutine(ResetProcessingFlag());
    }
    
    private System.Collections.IEnumerator ResetProcessingFlag()
    {
        yield return new UnityEngine.WaitForSeconds(0.1f); // 0.1초 대기
        isProcessing = false;
    }
}
