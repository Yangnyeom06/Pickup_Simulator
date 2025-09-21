using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DayManager : MonoBehaviour
{
    [Header("Saved Data")]
    public float timerTime;
    public int year;
    public int month;
    public int day;
    public TimeOfDay timeOfDay;
    public int pickUpItemCounts;
    public int sellItemCounts;
    public int todayGetMoney;
    public int todaySpendMoney;

    [Header("Don't Saved Data")]
    public GameObject player;
    public GameObject timer;
    public GameObject note;
    public PlayerManager playerManager;
    public TextMeshProUGUI today;
    public float rotationSpeed;
    public GameObject settleUi;
    public TextMeshProUGUI settleTodayText;
    public TextMeshProUGUI pickUpItemCountsText;
    public TextMeshProUGUI sellItemCountsText;
    public TextMeshProUGUI todayGetMoneyText;
    public TextMeshProUGUI todaySpendMoneyText;
    public TextMeshProUGUI ownMoney;
    private bool isTimerRunning = true;
    public ItemRaycast itemRaycast;
    bool noteActive = false;
    public FadeInOut FadeInout;
    
    [Header("Spawn Managers")]
    public UsedCarSpawnManager usedCarSpawnManager;

    void Start()
    {
        today.text = $"{month}/{day}";
        
        // UsedCarSpawnManager 자동 찾기
        if (usedCarSpawnManager == null)
        {
            usedCarSpawnManager = FindFirstObjectByType<UsedCarSpawnManager>();
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timerTime -= Time.deltaTime;

            timer.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if (timerTime <= 0f)
            {
                isTimerRunning = false;
                timerTime = 0f;
                Debug.Log("끝");
            }
        }
        if (noteActive == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(itemRaycast.mRayCamera.transform.position, itemRaycast.mRayCamera.transform.forward, out hit, itemRaycast.mRayDistance))
                {
                    if (hit.transform.gameObject == note.gameObject)
                    {
                        Open();
                    }
                }
            }
        }
    }

    private int[] daysInMonth = {
        31, 30, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
    };

    public void AdvanceTime()
    {
        timeOfDay = TimeOfDay.Day;
        day++;

        int maxDays = daysInMonth[month - 1];
        if (day > maxDays)
        {
            day = 1;
            month += 1;

            if (month > 12)
            {
                month = 1;
                year += 1;
            }
        }
        today.text = $"{month}/{day}";
    }

    public void StartDay()
    {
        player.transform.position = new Vector3(0, 2, 0);
        playerManager.healthLevel.current = playerManager.healthLevel.max;
        playerManager.staminaLevel.current = playerManager.staminaLevel.max;
        playerManager.StartHealthLoss();
        noteActive = false;
        note.SetActive(false);
        AdvanceTime();
        pickUpItemCounts = 0;
        sellItemCounts = 0;
        todayGetMoney = 0;
        todaySpendMoney = 0;
        Vector3 angles = timer.transform.eulerAngles;
        angles.z = 0f;
        timer.transform.eulerAngles = angles;
        timerTime = 1000000f;
        
        // 중고트럭 스폰 체크
        CheckUsedCarTruckSpawn();
    }

    public void End()
    {
        FadeInout.StartFadeInAndOut(() =>
        {
            player.transform.position = note.transform.position;
            noteActive = true;
            note.SetActive(true);
        });
    }

    public void Settle()
    {
        pickUpItemCountsText.text = pickUpItemCounts.ToString();
        sellItemCountsText.text = sellItemCounts.ToString();
        todayGetMoneyText.text = todayGetMoney.ToString();
        todaySpendMoneyText.text = todaySpendMoney.ToString();
        ownMoney.text = playerManager.money.ToString();
        settleTodayText.text = $"{year}년 {month}월 {day}일";
    }

    public void DaytoEvening()
    {
        if (timeOfDay == TimeOfDay.Day)
        {
            timeOfDay = TimeOfDay.Evening;
        }
    }

    public void Open()
    {
        Settle();
        settleUi.SetActive(true);
    }

    public void Exit()
    {
        settleUi.SetActive(false);
        Invoke("StartDay", 1.0f);
    }

    public void ResetDayData()
    {
        year = 2013;
        month = 3;
        day = 7;
        pickUpItemCounts = 0;
        sellItemCounts = 0;
        todayGetMoney = 0;
        todaySpendMoney = 0;
        Vector3 angles = timer.transform.eulerAngles;
        angles.z = 0f;
        timer.transform.eulerAngles = angles;
        timerTime = 1000000f;
        today.text = $"{month}/{day}";
    }
    
    /// <summary>
    /// 중고트럭 스폰 여부를 체크하고 결정
    /// </summary>
    private void CheckUsedCarTruckSpawn()
    {
        if (usedCarSpawnManager != null)
        {
            // 현재 날짜 계산 (게임 시작일부터의 경과 일수)
            int totalDays = CalculateTotalDays();
            usedCarSpawnManager.OnNewDayStarted(totalDays);
        }
        else
        {
            Debug.LogWarning("[DayManager] UsedCarSpawnManager가 할당되지 않았습니다!");
        }
    }
    
    /// <summary>
    /// 게임 시작일부터의 총 경과 일수 계산
    /// </summary>
    /// <returns>총 경과 일수</returns>
    public int CalculateTotalDays()
    {
        // 기준일: 2013년 3월 7일 (게임 시작일)
        int baseYear = 2013;
        int baseMonth = 3;
        int baseDay = 7;
        
        int totalDays = 0;
        
        // 연도 차이 계산
        for (int y = baseYear; y < year; y++)
        {
            totalDays += IsLeapYear(y) ? 366 : 365;
        }
        
        // 월 차이 계산 (현재 연도 내에서)
        for (int m = (year == baseYear ? baseMonth : 1); m < month; m++)
        {
            totalDays += daysInMonth[m - 1];
            
            // 윤년의 2월 처리
            if (m == 2 && IsLeapYear(year))
            {
                totalDays += 1;
            }
        }
        
        // 일 차이 계산
        if (year == baseYear && month == baseMonth)
        {
            totalDays += (day - baseDay);
        }
        else
        {
            totalDays += day - 1; // 현재 월의 1일부터 계산
        }
        
        return totalDays;
    }
    
    /// <summary>
    /// 윤년 여부 확인
    /// </summary>
    /// <param name="year">확인할 연도</param>
    /// <returns>윤년 여부</returns>
    private bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }
    
    // 싱글톤 패턴 추가
    public static DayManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("[DayManager] 여러 개의 DayManager가 존재합니다!");
        }
    }
}