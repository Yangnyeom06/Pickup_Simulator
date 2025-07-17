using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuantityDialog : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text quantityText;
    public Button   plusButton;
    public Button   minusButton;

    private ISaleSystem saleSystem;
    private ItemData    itemData;
    private int         maxQuantity;

    // 첫 번째 파라미터를 ISaleSystem, 두 번째 ItemData, 세 번째 int 순으로
    public void Setup(ISaleSystem system, ItemData data, int maxQty)
    {
        saleSystem   = system;
        itemData     = data;
        maxQuantity  = maxQty;
        saleSystem.selectedQuantity = 1;
        UpdateQuantity(saleSystem.selectedQuantity);    

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
        UpdateQuantity(saleSystem.selectedQuantity);
    }

    /// <summary>
    /// (4) – 버튼 클릭 시
    /// </summary>
    public void OnMinusButtonClicked()
    {
        saleSystem.DecreaseQuantity();
        UpdateQuantity(saleSystem.selectedQuantity);
    }

    /// <summary>
    /// 화면에 수량 표시를 갱신
    /// </summary>
    public void UpdateQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
        minusButton.interactable = quantity > 1;
        plusButton.interactable = quantity < maxQuantity;

    }
}