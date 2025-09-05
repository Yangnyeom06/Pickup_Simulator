using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

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
    public FadeInOut FadeInout;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        today.text = $"{month}/{day}";
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
        PlayerManager.Instance.healthLevel.current = PlayerManager.Instance.healthLevel.max;
        PlayerManager.Instance.staminaLevel.current = PlayerManager.Instance.staminaLevel.max;
        PlayerManager.Instance.StartHealthLoss();
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
    }

    public void End()
    {
        FadeInout.StartFadeInAndOut(() =>
        {
            player.transform.position = note.transform.position;
            note.SetActive(true);
        });
    }

    public void Settle()
    {
        pickUpItemCountsText.text = pickUpItemCounts.ToString();
        sellItemCountsText.text = sellItemCounts.ToString();
        todayGetMoneyText.text = todayGetMoney.ToString();
        todaySpendMoneyText.text = todaySpendMoney.ToString();
        ownMoney.text = PlayerManager.Instance.money.ToString();
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
}