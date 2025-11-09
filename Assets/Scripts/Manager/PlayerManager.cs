using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
=======
using System;

// 버프 정보 클래스
[System.Serializable]
public class BuffInfo
{
    public SnackEffectType buffType;
    public float value; // 버프 강도 (퍼센트)
    public float duration; // 남은 지속시간
    public string buffName; // 버프 이름 (디버깅용)
}
>>>>>>> 74b4bcf0 (update)

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

<<<<<<< HEAD
=======
    private float _baseHealthLossSpeed;
    private float _baseStaminaLossSpeed;
    [SerializeField] private float defaultHealthLossSpeed = 1f;
    [SerializeField] private float defaultStaminaLossSpeed = 1f;


>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD
=======
    
    // 버프 관리
    private Dictionary<SnackEffectType, BuffInfo> activeBuffs = new Dictionary<SnackEffectType, BuffInfo>();
    private Coroutine buffUpdateCoroutine;
    private float baseHealthLossSpeed;
    private float baseStaminaLossSpeed;
>>>>>>> 74b4bcf0 (update)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
<<<<<<< HEAD
    }

    private void Start()
    {
        StartHealthLoss();
=======
        DontDestroyOnLoad(gameObject); 

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
        baseHealthLossSpeed = HealthLossSpeed;
        baseStaminaLossSpeed = staminaLossSpeed;
        StartHealthLoss();
        StartBuffUpdate();
>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD
            healthLevel.current -= HealthLossSpeed;
=======
            // 버프 적용된 체력 소모량 계산
            float actualHealthLoss = GetActualHealthLossSpeed();
            healthLevel.current -= actualHealthLoss;
>>>>>>> 74b4bcf0 (update)
            yield return new WaitForSeconds(HealthLossInterval);
        }
        // 집으로 보내는 이벤트 실행

        // 나중에 바꿔주기
        DayManager.Instance.End();
        Debug.Log("체력이 0이 되었습니다!");
    }
<<<<<<< HEAD
=======
    
    /// <summary>
    /// 버프가 적용된 실제 체력 소모량을 반환합니다
    /// </summary>
    private float GetActualHealthLossSpeed()
    {
        if (activeBuffs.ContainsKey(SnackEffectType.HealthLossReduction))
        {
            float reductionPercent = activeBuffs[SnackEffectType.HealthLossReduction].value;
            return baseHealthLossSpeed * (1f - reductionPercent / 100f);
        }
        return baseHealthLossSpeed;
    }
>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD
            staminaLevel.current -= (int)staminaLossSpeed;
=======
            // 버프 적용된 스테미나 소모량 계산
            float actualStaminaLoss = GetActualStaminaLossSpeed();
            staminaLevel.current -= (int)actualStaminaLoss;
>>>>>>> 74b4bcf0 (update)
            yield return new WaitForSeconds(staminaLossInterval);

            // 스태미나가 0이 되면 자동으로 달리기 취소해야 하므로 이벤트나 상태 전달 필요
        }
        playerController.RunningCancel();
        Debug.Log("스태미나가 0이 되었습니다!");
    }
<<<<<<< HEAD
=======
    
    /// <summary>
    /// 버프가 적용된 실제 스테미나 소모량을 반환합니다
    /// </summary>
    private float GetActualStaminaLossSpeed()
    {
        if (activeBuffs.ContainsKey(SnackEffectType.StaminaLossReduction))
        {
            float reductionPercent = activeBuffs[SnackEffectType.StaminaLossReduction].value;
            return baseStaminaLossSpeed * (1f - reductionPercent / 100f);
        }
        return baseStaminaLossSpeed;
    }
>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD
    }
=======
        
        // 모든 버프 제거
        ClearAllBuffs();
    }
    
    #region BuffManagement
    /// <summary>
    /// 버프를 적용합니다
    /// </summary>
    /// <param name="buffType">버프 타입</param>
    /// <param name="value">버프 강도 (퍼센트)</param>
    /// <param name="duration">지속시간 (초)</param>
    /// <param name="buffName">버프 이름</param>
    public void ApplyBuff(SnackEffectType buffType, float value, float duration, string buffName = "")
    {
        // 같은 타입의 버프가 이미 있으면 덮어쓰기
        if (activeBuffs.ContainsKey(buffType))
        {
            activeBuffs[buffType].value = value;
            activeBuffs[buffType].duration = duration;
            activeBuffs[buffType].buffName = buffName;
            Debug.Log($"[버프 갱신] {buffName}: {value}%, {duration}초");
        }
        else
        {
            activeBuffs[buffType] = new BuffInfo
            {
                buffType = buffType,
                value = value,
                duration = duration,
                buffName = buffName
            };
            Debug.Log($"[버프 적용] {buffName}: {value}%, {duration}초");
            
            // 체력/스테미나 소모량 버프인 경우 코루틴 재시작하여 즉시 반영
            if (buffType == SnackEffectType.HealthLossReduction && healthLossCoroutine != null)
            {
                StartHealthLoss(); // 재시작하여 새 값 적용
            }
            else if (buffType == SnackEffectType.StaminaLossReduction && staminaLossCoroutine != null)
            {
                StopStaminaLoss();
                if (playerController != null && playerController.IsRunning())
                {
                    StartStaminaLoss(); // 달리는 중이면 재시작
                }
            }
        }
        
        // 버프 업데이트 시작 (이미 실행 중이면 자동으로 무시됨)
        StartBuffUpdate();
    }
    
    /// <summary>
    /// 버프를 제거합니다
    /// </summary>
    /// <param name="buffType">제거할 버프 타입</param>
    public void RemoveBuff(SnackEffectType buffType)
    {
        if (activeBuffs.ContainsKey(buffType))
        {
            string buffName = activeBuffs[buffType].buffName;
            activeBuffs.Remove(buffType);
            Debug.Log($"[버프 제거] {buffName}");
            
            // 체력/스테미나 소모량 버프 제거 시 코루틴 재시작
            if (buffType == SnackEffectType.HealthLossReduction && healthLossCoroutine != null)
            {
                StartHealthLoss(); // 재시작하여 기본 값 복원
            }
            else if (buffType == SnackEffectType.StaminaLossReduction && staminaLossCoroutine != null)
            {
                StopStaminaLoss();
                if (playerController != null && playerController.IsRunning())
                {
                    StartStaminaLoss(); // 달리는 중이면 재시작
                }
            }
        }
    }
    
    /// <summary>
    /// 모든 버프를 제거합니다
    /// </summary>
    public void ClearAllBuffs()
    {
        activeBuffs.Clear();
        Debug.Log("[버프] 모든 버프가 제거되었습니다.");
    }
    
    /// <summary>
    /// 특정 타입의 버프가 활성화되어 있는지 확인합니다
    /// </summary>
    public bool HasBuff(SnackEffectType buffType)
    {
        return activeBuffs.ContainsKey(buffType);
    }
    
    /// <summary>
    /// 특정 타입의 버프 값을 가져옵니다
    /// </summary>
    public float GetBuffValue(SnackEffectType buffType)
    {
        if (activeBuffs.ContainsKey(buffType))
        {
            return activeBuffs[buffType].value;
        }
        return 0f;
    }
    
    /// <summary>
    /// 달리기 속도 증가 버프의 배율을 반환합니다 (1.0 = 기본 속도)
    /// </summary>
    public float GetRunSpeedMultiplier()
    {
        if (activeBuffs.ContainsKey(SnackEffectType.RunSpeedBoost))
        {
            float boostPercent = activeBuffs[SnackEffectType.RunSpeedBoost].value;
            return 1f + (boostPercent / 100f);
        }
        return 1f;
    }
    
    /// <summary>
    /// 버프 업데이트 코루틴 시작
    /// </summary>
    private void StartBuffUpdate()
    {
        if (buffUpdateCoroutine != null) return;
        buffUpdateCoroutine = StartCoroutine(UpdateBuffs());
    }
    
    /// <summary>
    /// 버프 지속시간을 업데이트하고 만료된 버프를 제거합니다
    /// </summary>
    private IEnumerator UpdateBuffs()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1초마다 체크
            
            List<SnackEffectType> expiredBuffs = new List<SnackEffectType>();
            
            foreach (var buff in activeBuffs.Values)
            {
                buff.duration -= 1f;
                
                if (buff.duration <= 0f)
                {
                    expiredBuffs.Add(buff.buffType);
                }
            }
            
            // 만료된 버프 제거
            foreach (var buffType in expiredBuffs)
            {
                RemoveBuff(buffType);
            }
            
            // 모든 버프가 없으면 코루틴 종료
            if (activeBuffs.Count == 0)
            {
                buffUpdateCoroutine = null;
                yield break;
            }
        }
    }
    #endregion
>>>>>>> 74b4bcf0 (update)
}