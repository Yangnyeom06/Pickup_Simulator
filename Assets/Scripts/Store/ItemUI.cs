using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ShopItemData shopItemData; // 이 슬롯에 들어있는 아이템 정보
    public Image iconImage;   // 아이템 아이콘 이미지
    public TMP_Text countText; // 아이템 수량 텍스트 (필요시)

    private Transform originalParent;
    private CanvasGroup canvasGroup;

    public string itemName;
    public int sellPrice; // ← 이게 정의되어 있어야 함

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetItem(ShopItemData shopItemData, int count = 1)
    {
        this.shopItemData = shopItemData;
        iconImage.sprite = shopItemData.icon;
        countText.text = count > 1 ? count.ToString() : "";
        gameObject.SetActive(true);
    }

    public void ClearItem()
    {
        shopItemData = null;
        iconImage.sprite = null;
        countText.text = "";
        gameObject.SetActive(false);
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // UI 최상위로 이동 (Canvas)
        canvasGroup.blocksRaycasts = false;  // 드롭 감지 위해 Raycast 차단
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }
}
