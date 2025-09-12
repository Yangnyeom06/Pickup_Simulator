using Unity.VisualScripting;
using UnityEngine;


public class Snack : MonoBehaviour
{
    public SnackItemData SnackItemData; // 아이템 데이터 참조
    InventoryManager inventoryManager;
/*
    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null)
        {
            if (inventoryManager.AddItem(itemData)) // 성공한 경우만 삭제
            {
                DeleteItem();
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
*/
    public void SetSnackItemData(SnackItemData data)
    {
        SnackItemData = data;
    }

    public void GotSnackItem()
    {
        inventoryManager.AddSnack(SnackItemData);
    }

    public void DeleteSnackItem()
    {
        Destroy(gameObject);
    }
}