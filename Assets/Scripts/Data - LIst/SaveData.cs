using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public InventoryData inventoryData;
    public GameDate gameData;
    public List<UISaveData> uiObjects = new();
    public List<ItemInstanceData> collectedItemsData = new();
}