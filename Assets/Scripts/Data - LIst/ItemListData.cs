using UnityEngine;

[CreateAssetMenu(fileName = "NewItemListData", menuName = "Item/ItemListData")]
public class ItemListData : ScriptableObject
{
    public Item[] roadItems; // 길에서 드랍되는 아이템 리스트 
    public Item[] mountainItems; // 산에서 드랍되는 아이템 리스트 
    public Item[] elementSchoolItems; // 초등학교에서 드랍되는 아이템 리스트 
    public Item[] middleSchoolItems; // 중학교에서 드랍되는 아이템 리스트
    public Item[] cityItems; //도시에서 드랍되는 아이템 리스트
}