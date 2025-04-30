using UnityEngine;

public class PlayerStats : MonoBehaviour // 후에 이 코드는 PlayerManager에 옮기기
{
    public IntValueSO healthLevel;
    public IntValueSO staminaLevel;
    public IntValueSO speedLevel;
    public IntValueSO inventoryLevel;
    public IntValueSO mapLevel;
    public int health;
    public int satmina;
    public int speed;

/*
    void Start()
    {
        healthLevel.Register(OnHealthLevelChanged); // 함수 등록
        staminaLevel.Register(OnStaminaLevelChanged); // 함수 등록

    }

    // 체력 레벨이 바뀔 때 호출되는 함수
    void OnHealthLevelChanged(int newValue)
    {
        Debug.Log($"Health Level 바뀜: {newValue}");
    }

    // 스태미나 레벨이 바뀔 때 호출되는 함수
    void OnStaminaLevelChanged(int newValue)
    {
        Debug.Log($"Stamina Level 바뀜: {newValue}");
    }

    // 필요시 함수 해제
    void OnDestroy()
    {
        healthLevel.Unregister(OnHealthLevelChanged);
        staminaLevel.Unregister(OnStaminaLevelChanged);
    }*/
}