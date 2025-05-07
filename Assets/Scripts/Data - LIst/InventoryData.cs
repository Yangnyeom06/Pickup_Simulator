using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public int inventorySlotCount;

    public InventoryData(int inventorySlotCount)
    {
        this.inventorySlotCount = inventorySlotCount;
    }

    public static InventoryData FromData(InventoryManager inventory)
    {
        return new InventoryData(
            inventory.inventorySlotCount.Value
        );
    }

    public void ApplyToInventory(InventoryManager inventory)
    {
        inventory.inventorySlotCount.Value = inventorySlotCount;
    }
}