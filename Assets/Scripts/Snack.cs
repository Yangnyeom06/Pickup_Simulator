using Unity.VisualScripting;
using UnityEngine;


public class Snack : MonoBehaviour
{
    public SnackData snackData; // 아이템 데이터 참조
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
    public void SetSnackData(SnackData data)
    {
        snackData = data;
    }

    public void GotSnackItem()
    {
        bool success = inventoryManager.AddSnack(snackData);
        if (!success)
        {
            Debug.LogWarning($"인벤토리가 가득 차서 {snackData.snackName}을(를) 추가할 수 없습니다!");
        }
    }

    public void DeleteSnackItem()
    {
        Destroy(gameObject);
    }
}