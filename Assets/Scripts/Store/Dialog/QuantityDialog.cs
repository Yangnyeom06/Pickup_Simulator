using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuantityDialog : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text quantityText;
    public Button   plusButton;
    public Button   minusButton;

    private SaleSystem saleSystem;
    private ItemData itemData;
    private int        maxQuantity;

    /// <summary>
    /// (3) SaleSystem.OnSlotClicked()에서 반드시 한 번 호출
    /// </summary>
    public void Setup(SaleSystem system, ItemData data, int maxQty)
    {
        saleSystem   = system;
        itemData = data;
        maxQuantity  = maxQty;
        saleSystem.selectedQuantity = 1;

        quantityText.text = saleSystem.selectedQuantity.ToString();

        // 버튼 리스너 바인딩 (프리팹 인스펙터 OnClick은 모두 지워두세요)
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(OnPlusButtonClicked);

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    /// <summary>
    /// (4) + 버튼 클릭 시
    /// </summary>
    public void OnPlusButtonClicked()
    {
        saleSystem.IncreaseQuantity(maxQuantity);
        quantityText.text = saleSystem.selectedQuantity.ToString();
    }

    /// <summary>
    /// (4) – 버튼 클릭 시
    /// </summary>
    public void OnMinusButtonClicked()
    {
        saleSystem.DecreaseQuantity();
        quantityText.text = saleSystem.selectedQuantity.ToString();
    }

    /// <summary>
    /// 화면에 수량 표시를 갱신
    /// </summary>
    public void UpdateQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
    }
}