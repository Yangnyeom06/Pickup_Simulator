using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int healthLevel;
    public int staminaLevel;
    public int speedLevel;
    public int inventoryLevel;
    public int mapLevel;
    public float health;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
    public int speed;
    public int money;

    public PlayerData(int healthLevel, int staminaLevel, int speedLevel, int inventoryLevel, int mapLevel, float health, float maxHealth, float stamina, float maxStamina, int speed, int money)
    {
        this.healthLevel = healthLevel;
        this.staminaLevel = staminaLevel;
        this.speedLevel = speedLevel;
        this.inventoryLevel = inventoryLevel;
        this.mapLevel = mapLevel;
        this.health = health;
        this.maxHealth = maxHealth;
        this.stamina = stamina;
        this.maxStamina = maxStamina;
        this.speed = speed;
        this.money = money;
    }

    public static PlayerData FromData(PlayerManager stats)
    {
        return new PlayerData(
            stats.healthLevel.Value,
            stats.staminaLevel.Value,
            stats.speedLevel.Value,
            stats.inventoryLevel.Value,
            stats.mapLevel.Value,
            stats.healthLevel.current,
            stats.healthLevel.max,
            stats.staminaLevel.current,
            stats.staminaLevel.max,
            stats.speedLevel.Value,
            stats.money
        );
    }

    public void ApplyToPlayer(PlayerManager stats)
    {
        stats.healthLevel.Value = healthLevel;
        stats.staminaLevel.Value = staminaLevel;
        stats.speedLevel.Value = speedLevel;
        stats.inventoryLevel.Value = inventoryLevel;
        stats.mapLevel.Value = mapLevel;
        stats.healthLevel.current = health;
        stats.healthLevel.max = maxHealth;
        stats.staminaLevel.current = stamina;
        stats.staminaLevel.max = maxStamina;
        stats.money = money;
    }
}