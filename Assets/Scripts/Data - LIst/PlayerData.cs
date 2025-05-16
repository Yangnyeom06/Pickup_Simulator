using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int healthLevel;
    public int staminaLevel;
    public int speedLevel;
    public int inventoryLevel;
    public int mapLevel;
    public int health;
    public int stamina;
    public int speed;
    public int money;

    public PlayerData(int healthLevel, int staminaLevel, int speedLevel, int inventoryLevel, int mapLevel, int health, int stamina, int speed, int money)
    {
        this.healthLevel = healthLevel;
        this.staminaLevel = staminaLevel;
        this.speedLevel = speedLevel;
        this.inventoryLevel = inventoryLevel;
        this.mapLevel = mapLevel;
        this.health = health;
        this.stamina = stamina;
        this.speed = speed;
        this.money = money;
    }

    public static PlayerData FromData(PlayerStats stats)
    {
        return new PlayerData(
            stats.healthLevel.Value,
            stats.staminaLevel.Value,
            stats.speedLevel.Value,
            stats.inventoryLevel.Value,
            stats.mapLevel.Value,
            stats.health,
            stats.stamina,
            stats.speed,
            stats.money
        );
    }

    public void ApplyToStats(PlayerStats stats)
    {
        stats.healthLevel.Value = healthLevel;
        stats.staminaLevel.Value = staminaLevel;
        stats.speedLevel.Value = speedLevel;
        stats.inventoryLevel.Value = inventoryLevel;
        stats.mapLevel.Value = mapLevel;
        stats.health = health;
        stats.stamina = stamina;
        stats.speed = speed;
        stats.money = money;
    }
}