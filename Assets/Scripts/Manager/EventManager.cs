using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private Dictionary<(int year, int month, int day), Action> scheduledEvents
        = new Dictionary<(int, int, int), Action>();


void Start()
{
    // 2013년 3월 10일에 이벤트 실행
    EventManager.Instance.ScheduleEvent(2013, 3, 10, () =>
    {
        Debug.Log("3월 10일 이벤트 발생!");
        // 원하는 함수 호출 가능
        SomeSpecialFunction();
    });
}

void SomeSpecialFunction()
{
    Debug.Log("특수 함수 실행!");
}
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 특정 날짜에 실행할 이벤트 등록
    /// </summary>
    public void ScheduleEvent(int year, int month, int day, Action action)
    {
        var key = (year, month, day);
        if (!scheduledEvents.ContainsKey(key))
        {
            scheduledEvents.Add(key, action);
        }
        else
        {
            scheduledEvents[key] += action; // 여러 개 등록 가능
        }
    }

    /// <summary>
    /// 해당 날짜에 도착했을 때 이벤트 실행
    /// </summary>
    public void TriggerEvent(int year, int month, int day)
    {
        var key = (year, month, day);
        if (scheduledEvents.TryGetValue(key, out Action action))
        {
            action?.Invoke();
            Debug.Log($"이벤트 실행: {year}/{month}/{day}");
        }
    }
    

}