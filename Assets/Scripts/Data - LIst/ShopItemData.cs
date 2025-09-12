using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Store/Shop Item")]
public class ShopItemData : ScriptableObject
{
    public string itemID;               // 고유 식별자 (SnackItemData와 일관성)
    public string itemName;             // 상점에 표시될 이름
    public Sprite icon;                 // 이미지
    public int price;                   // 가격
    public string description;          // 설명
    public ItemType itemType;           // 물티슈 등
    public string itemCode;             // 내부 식별용 코드 (예: "drink_cola") - 하위 호환성
    public int slotNum;                 // 슬롯 번호
}