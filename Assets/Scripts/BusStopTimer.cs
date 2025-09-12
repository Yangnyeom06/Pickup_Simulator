using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class BusTimer : MonoBehaviour
{
    public float timer = 10f;           // 제한 시간 (초)
    public GameObject spawnPrefab;      // 시간 끝나면 생성할 오브젝트
    public Transform spawnPoint;        // 생성 위치
    public Button startButton;          // 시작 버튼
    public TextMeshProUGUI timerText;         // 남은 시간을 표시할 UI 텍스트

    private bool isTimerRunning = false;
    private float currentTime;

    void Start()
    {
        currentTime = timer;
        startButton.onClick.AddListener(StartTimer); // 버튼 이벤트 연결
        UpdateTimerUI();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0f)
            {
                isTimerRunning = false;
                currentTime = 0f;
                UpdateTimerUI();

                // 특정 오브젝트 생성
                if (spawnPrefab != null && spawnPoint != null)
                {
                    Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
                }

                Debug.Log("버스 떠남! (오브젝트 생성됨)");
            }
        }
    }

    public void StartTimer()
    {
        currentTime = timer;
        isTimerRunning = true;
        UpdateTimerUI();
        Debug.Log("버스타이머 시작!");
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "남은 시간: " + Mathf.Ceil(currentTime).ToString() + "초";
        }
    }
}