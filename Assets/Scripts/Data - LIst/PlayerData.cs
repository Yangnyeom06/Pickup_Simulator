using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int healthLevel;
    public int staminaLevel;
    public int speedLevel;
    public int inventoryLevel;
    public int mapLevel;

    public PlayerData(int healthLevel, int staminaLevel, int speedLevel, int inventoryLevel, int mapLevel)
    {
        this.healthLevel = healthLevel;
        this.staminaLevel = staminaLevel;
        this.speedLevel = speedLevel;
        this.inventoryLevel = inventoryLevel;
        this.mapLevel = mapLevel;
    }

    public static PlayerData FromData(PlayerStats stats)
    {
        return new PlayerData(
            stats.healthLevel.Value,
            stats.staminaLevel.Value,
            stats.speedLevel.Value,
            stats.inventoryLevel.Value,
            stats.mapLevel.Value
        );
    }

    public void ApplyToStats(PlayerStats stats)
    {
        stats.healthLevel.Value = healthLevel;
        stats.staminaLevel.Value = staminaLevel;
        stats.speedLevel.Value = speedLevel;
        stats.inventoryLevel.Value = inventoryLevel;
        stats.mapLevel.Value = mapLevel;
    }
}