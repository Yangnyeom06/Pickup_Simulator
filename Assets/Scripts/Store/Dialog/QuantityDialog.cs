using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuantityDialog : MonoBehaviour
{
    // 다이얼로그 UI에 표시될 텍스트와 버튼들을 Unity 에디터에서 연결결
    public TMP_Text itemNameText;
    public TMP_Text quantityText;
    public Button plusButton;
    public Button minusButton;
    public Button sellButton;

    // 판매 시스템, 아이템 정보, 최대 선택 가능한 수량을 저장장
    private SellSystem sellSystem;
    private ItemData itemData;
    private int maxQuantity;

    // 다이얼로그 UI 초기화 및 버튼 이벤트 연결
    public void Setup(SellSystem system, ItemData item, int maxQty)
    {
        sellSystem = system;
        itemData = item;
        maxQuantity = maxQty;
        itemNameText.text = item.itemName;
        quantityText.text = "1"; // 초기 수량은 1

        // + 버튼 클릭 시 수량 증가
        plusButton.onClick.AddListener(() => {
        sellSystem.IncreaseQuantity(maxQuantity);
        quantityText.text = sellSystem.selectedQuantity.ToString();
        });

        // - 버튼 클릭 시 수량 감소
        minusButton.onClick.AddListener(() => {
        sellSystem.DecreaseQuantity();
        quantityText.text = sellSystem.selectedQuantity.ToString();
    });

        // 판매 버튼 클릭 시 판매 처리
        sellButton.onClick.AddListener(() => sellSystem.OnSellButtonClicked());
    }
} 