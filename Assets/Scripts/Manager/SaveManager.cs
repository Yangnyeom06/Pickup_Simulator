using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class SaveManager : MonoBehaviour
{
    // SaveManager의 유일한 인스턴스를 보장
    public static SaveManager Instance { get; private set; }

    // 현재 게임 데이터를 담을 SaveData 객체
    public SaveData currentSaveData = new();

    // 필요한 게임 데이터들
    public PlayerStats playerStats;
    public InventoryManager inventory;

    public int slotId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경돼도 이 오브젝트는 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 새로 생성된 오브젝트는 파괴
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private string GetSavePath(int slotId)
    {
        return Application.persistentDataPath + $"/gameSave_slot_{slotId}.json";
    }

    // 게임 데이터를 JSON으로 저장하는 함수
    public void SaveGame(int slotId)
    {
        // PlayerStats의 최신 상태를 currentSaveData에 반영
        currentSaveData.playerData = PlayerData.FromData(playerStats);
        currentSaveData.inventoryData = InventoryData.FromData(inventory);

        // currentSaveData를 JSON 형식으로 변환
        string json = JsonUtility.ToJson(currentSaveData, true);

        // 파일로 저장
        File.WriteAllText(GetSavePath(slotId), json);
        Debug.Log($"세이브 완료 (슬롯 {slotId})");
    }

    // 게임 데이터를 JSON에서 불러오는 함수
    public void LoadGame(int slotId)
    {
        string savePath = GetSavePath(slotId);
        if (File.Exists(savePath))
        {
            // JSON 파일을 읽어 currentSaveData로 로드
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);

            // PlayerStats에 로드된 데이터를 반영
            currentSaveData.playerData.ApplyToStats(playerStats);

            // InventoryManager에 로드된 데이터를 반영
            currentSaveData.inventoryData.ApplyToInventory(inventory);

            Debug.Log($"로드 완료 (슬롯 {slotId})");
        }
        else
        {
            Debug.LogWarning($"세이브 파일 없음: 슬롯 {slotId}");
        }
    }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene1234") // PlayScene 이름으로 확인
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            inventory = FindFirstObjectByType<InventoryManager>();

            if (playerStats != null && inventory != null)
            {
                slotId = SceneChangeManager.Instance.selectSlotId;
                // 로드된 데이터 반영
                LoadGame(slotId);

                Debug.Log($"{scene.name}씬 로드 후 {slotId}번 데이터 적용 완료");
            }
            else
            {
                Debug.LogWarning($"{scene.name}에서 필요한 컴포넌트를 찾지 못했습니다.");
            }
        }
    }

    public void ResetAllData(int slotId)
    {
        Debug.Log(slotId);
        playerStats.ResetStats();
        inventory.ResetSlots();


        // 저장된 파일도 삭제 (SaveManager가 save.json 등에 저장한다고 가정)
        DeleteSaveFile(slotId);

        // 새로 저장
        SaveGame(slotId);

        Debug.Log($"{slotId}번 세이브 파일 리셋 완료");
    }

    public void DeleteSaveFile(int slotId)
    {
        if (File.Exists(GetSavePath(slotId)))
        {
            File.Delete(GetSavePath(slotId));
            Debug.Log($"{slotId}번 세이브 파일 삭제됨");
        }
        else
        {
            Debug.Log($"삭제할 {slotId}번세이브 파일이 없음");
        }
    }

    public void LogThisObject()
    {
        Debug.Log($"[Object Log] 이름: {gameObject.name}");
    }
}