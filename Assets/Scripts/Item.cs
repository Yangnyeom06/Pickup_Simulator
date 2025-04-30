using UnityEngine;


public class Item : MonoBehaviour
{
    public ItemData itemData; // 아이템 데이터 참조

    public void SetItemData(ItemData data)
    {
        itemData = data;
    }
}