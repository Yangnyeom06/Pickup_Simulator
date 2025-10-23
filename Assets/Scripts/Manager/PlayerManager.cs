using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public PlayerController playerController;
    public IntStatValueSO healthLevel;
    public IntStatValueSO staminaLevel;
    public IntStatValueSO speedLevel;
    public IntStatValueSO inventoryLevel;
    public IntStatValueSO mapLevel;
    public PlayerData playerData;
    private MoneyManager moneyManager;

    public int money
    {
        get => playerData.money;
        set { playerData.money = value; MoneyManager.Instance.UpdateMoneyUI(); }
    }
    private Coroutine healthLossCoroutine;
    public float HealthLossInterval = 1f;
    public float HealthLossSpeed = 1f;
    private Coroutine staminaLossCoroutine;
    public float staminaLossInterval = 1;
    public float staminaLossSpeed = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }


    private void Start()
    {
        StartHealthLoss();
    }

    #region HealthLoss
    public void StartHealthLoss()
    {
        if (healthLossCoroutine != null)
            StopCoroutine(healthLossCoroutine);
        healthLossCoroutine = StartCoroutine(LossHealthOverTime());
    }

    public void StopHealthLoss()
    {
        if (healthLossCoroutine != null)
        {
            StopCoroutine(healthLossCoroutine);
            healthLossCoroutine = null;
        }
    }

    private IEnumerator LossHealthOverTime()
    {
        while (healthLevel.current > 0)
        {
            healthLevel.current -= HealthLossSpeed;
            yield return new WaitForSeconds(HealthLossInterval);
        }
        // 집으로 보내는 이벤트 실행

        // 나중에 바꿔주기
        DayManager.Instance.End();
        Debug.Log("체력이 0이 되었습니다!");
    }
    #endregion

    #region StaminaLoss
    public void StartStaminaLoss()
    {
        if (staminaLossCoroutine != null) return;

        staminaLossCoroutine = StartCoroutine(LossStaminaOverTime());
    }

    public void StopStaminaLoss()
    {
        if (staminaLossCoroutine != null)
        {
            StopCoroutine(staminaLossCoroutine);
            staminaLossCoroutine = null;
        }
    }

    private IEnumerator LossStaminaOverTime()
    {
        while (staminaLevel.current > 0)
        {
            staminaLevel.current -= (int)staminaLossSpeed;
            yield return new WaitForSeconds(staminaLossInterval);

            // 스태미나가 0이 되면 자동으로 달리기 취소해야 하므로 이벤트나 상태 전달 필요
        }
        playerController.RunningCancel();
        Debug.Log("스태미나가 0이 되었습니다!");
    }
    #endregion

    #region StaminaManagement
    /// <summary>
    /// 스낵 아이템을 사용하여 스태미나를 증가시킵니다
    /// </summary>
    /// <param name="amount">증가시킬 스태미나 양</param>
    /// <returns>실제로 증가한 스태미나 양</returns>
    public float RestoreStamina(float amount)
    {
        if (amount <= 0) return 0;

        float previousStamina = staminaLevel.current;
        float newStamina = Mathf.Min(staminaLevel.current + amount, staminaLevel.max);
        staminaLevel.current = newStamina;

        float actualIncrease = newStamina - previousStamina;
        Debug.Log($"스태미나 회복: +{actualIncrease} (현재: {newStamina}/{staminaLevel.max})");
        
        return actualIncrease;
    }

    /// <summary>
    /// 현재 스태미나가 최대치인지 확인
    /// </summary>
    /// <returns>스태미나가 최대치면 true</returns>
    public bool IsStaminaFull()
    {
        return staminaLevel.current >= staminaLevel.max;
    }
    #endregion

    #region HealthManagement
    /// <summary>
    /// 스낵 아이템을 사용하여 체력을 증가시킵니다
    /// </summary>
    /// <param name="amount">증가시킬 체력 양</param>
    /// <returns>실제로 증가한 체력 양</returns>
    public float RestoreHealth(float amount)
    {
        if (amount <= 0) return 0;

        float previousHealth = healthLevel.current;
        float newHealth = Mathf.Min(healthLevel.current + amount, healthLevel.max);
        healthLevel.current = newHealth;

        float actualIncrease = newHealth - previousHealth;
        Debug.Log($"체력 회복: +{actualIncrease} (현재: {newHealth}/{healthLevel.max})");

        return actualIncrease;
    }

    /// <summary>
    /// 현재 체력이 최대치인지 확인
    /// </summary>
    /// <returns>체력이 최대치면 true</returns>
    public bool IsHealthFull()
    {
        return healthLevel.current >= healthLevel.max;
    }
    #endregion

    public void ResetPlayerData()
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
        MoneyManager.Instance.UpdateMoneyUI();
    }
}