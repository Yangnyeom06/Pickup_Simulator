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

    private float _baseHealthLossSpeed;
    private float _baseStaminaLossSpeed;
    [SerializeField] private float defaultHealthLossSpeed = 1f;
    [SerializeField] private float defaultStaminaLossSpeed = 1f;

    public int money
    {
        get => playerData.money;
        set { playerData.money = value; MoneyManager.Instance.UpdateMoneyUI(); }
    }
    private float _staminaDrainBuffer = 0f;
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

        if (HealthLossSpeed <= 0f) HealthLossSpeed = defaultHealthLossSpeed;
        if (staminaLossSpeed <= 0f) staminaLossSpeed = defaultStaminaLossSpeed;
        if (HealthLossInterval <= 0f) HealthLossInterval = 1f;
        if (staminaLossInterval <= 0f) staminaLossInterval = 1f;

        // ✅ 기본값 백업(배율 적용의 기준)
        _baseHealthLossSpeed = HealthLossSpeed;
        _baseStaminaLossSpeed = staminaLossSpeed;

    }

    private void Start()
    {
        _baseHealthLossSpeed = HealthLossSpeed;
        _baseStaminaLossSpeed = staminaLossSpeed;
        StartHealthLoss();
        DebugPrintLossState("Start");
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
    public IEnumerator LossStaminaOverTime()
    {
        while (staminaLevel.current > 0)
        {
            _staminaDrainBuffer += staminaLossSpeed;

            int delta = Mathf.FloorToInt(_staminaDrainBuffer);
            if (delta >= 1)
            {
                staminaLevel.current -= delta;
                _staminaDrainBuffer -= delta;
            }

            yield return new WaitForSeconds(staminaLossInterval);
        }
        playerController.RunningCancel();
        Debug.Log("스태미나가 0이 되었습니다!");
    }


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
    // PlayerManager.cs 내부
    public void SetPassiveDrainMultipliers(float healthMul, float staminaMul)
    {
        // 1.0f = 원래 속도, 0.8f = 20% 감소(더 천천히 깎임)
        healthMul = Mathf.Clamp(healthMul, 0.1f, 2f);
        staminaMul = Mathf.Clamp(staminaMul, 0.1f, 2f);

        HealthLossSpeed = _baseHealthLossSpeed * healthMul;
        staminaLossSpeed = _baseStaminaLossSpeed * staminaMul;

        Debug.Log($"[PlayerManager] Passive drain x({healthMul}, {staminaMul}) → " +
                  $"HealthLossSpeed={HealthLossSpeed}, StaminaLossSpeed={staminaLossSpeed}");
    }

    private void DebugPrintLossState(string where)
    {
        Debug.Log($"[LossState@{where}] " +
                  $"HLoss: speed={HealthLossSpeed}, interval={HealthLossInterval}, current={healthLevel?.current} " +
                  $" | SLoss: speed={staminaLossSpeed}, interval={staminaLossInterval}, current={staminaLevel?.current} " +
                  $" | timeScale={Time.timeScale}");
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (HealthLossSpeed < 0.01f) HealthLossSpeed = 1f;
        if (staminaLossSpeed < 0.01f) staminaLossSpeed = 1f;
        if (HealthLossInterval <= 0f) HealthLossInterval = 1f;
        if (staminaLossInterval <= 0f) staminaLossInterval = 1f;
    }
#endif
}
#endregion