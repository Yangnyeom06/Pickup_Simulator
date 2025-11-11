using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class BusTimer : MonoBehaviour
{
    public static BusTimer Instance { get; private set; }
    public float timer = 10f;           // 제한 시간 (초)
    public GameObject spawnPrefab;      // 시간 끝나면 생성할 오브젝트
    public Transform spawnPoint;        // 생성 위치
    public Button startButton;          // 시작 버튼
    public TextMeshProUGUI timerText;         // 남은 시간을 표시할 UI 텍스트
    public bool saveItemCheck;

    private bool isTimerRunning = false;
    private float currentTime;


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

                if (saveItemCheck == true)
                {
                    saveItemCheck = false;
                    DialogueManager.Instance.CodeStartDialogue("예비 버스카드가 있어서 다행이었어...");
                    // 인보크로 씬 체인지 함수 사용, 함수 사용까지 3초 딜레이 주기
                }
                else if (saveItemCheck == false)
                {
                    DialogueManager.Instance.CodeStartDialogue("안돼! 버스를 놓쳤어!");
                    // 인보크로 씬 체인지 함수 사용, 함수 사용까지 3초 딜레이 주기
                    InventoryManager.Instance.ResetSlots();
                    DialogueManager.Instance.CodeStartDialogue("다 잃어버렸어...");
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