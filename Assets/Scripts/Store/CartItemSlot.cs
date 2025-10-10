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

    public void ItemSetup(ShopItemData data, int quantity, BuySystem system)
    {

        this.itemData = data;
        this.snackData = null;
        this.buySystem = system;

        itemNameText.text = $"{data.itemName}";
            
        // Setup 시에도 버튼 이벤트 재설정
        SetupDeleteButton();
    }

    public void SnackSetup(SnackData data, int quantity, BuySystem system)
    {

        this.snackData = data;
        this.itemData = null;    
        this.buySystem = system; 

        itemNameText.text = $"{data.snackName}";
        if (quantityText != null)
            quantityText.text = $"x{quantity}";
            
        // Setup 시에도 버튼 이벤트 재설정
        SetupDeleteButton();
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
        
        // 처리 완료 후 플래그 해제 (약간의 지연을 두어 중복 클릭 방지)
        StartCoroutine(ResetProcessingFlag());
    }
    
    private System.Collections.IEnumerator ResetProcessingFlag()
    {
        yield return new UnityEngine.WaitForSeconds(0.1f); // 0.1초 대기
        isProcessing = false;
    }
}
