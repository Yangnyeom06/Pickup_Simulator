using UnityEngine;

public class PlayerStats : MonoBehaviour // 후에 이 코드는 PlayerManager에 옮기기
{
    public IntStatValueSO healthLevel;
    public IntStatValueSO staminaLevel;
    public IntStatValueSO speedLevel;
    public IntStatValueSO inventoryLevel;
    public IntStatValueSO mapLevel;
    public int money;

    public void ResetStats()
    {
        healthLevel.Value = 0;
        staminaLevel.Value = 0;
        speedLevel.Value = 0;
        inventoryLevel.Value = 0;
        mapLevel.Value = 0;
        healthLevel.max = 100;
        staminaLevel.max = 50;
        healthLevel.current = 100;
        staminaLevel.current = 50;
        speedLevel.current = 1;
        money = 0;
    }

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