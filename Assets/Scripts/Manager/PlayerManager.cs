using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerController playerController;
    public IntStatValueSO healthLevel;
    public IntStatValueSO staminaLevel;
    public IntStatValueSO speedLevel;
    public IntStatValueSO inventoryLevel;
    public IntStatValueSO mapLevel;
    public int money;
    private Coroutine healthLossCoroutine;
    public float HealthLossInterval = 1f;
    public float HealthLossSpeed = 1f;
    private Coroutine staminaLossCoroutine;
    public float staminaLossInterval = 1;
    public float staminaLossSpeed = 1f;

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
    }
}