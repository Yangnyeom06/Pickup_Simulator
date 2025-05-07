using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    // SaveManager의 유일한 인스턴스를 보장
    public static SaveManager Instance { get; private set; }

    // 현재 게임 데이터를 담을 SaveData 객체
    public SaveData currentSaveData = new();

    // 필요한 게임 데이터들
    public PlayerStats playerStats;
    public InventoryManager inventory;

    private string savePath;

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

        savePath = Application.persistentDataPath + "/gameSave.json"; // 저장 경로 설정
    }

    // 게임 데이터를 JSON으로 저장하는 함수
    public void SaveGame()
    {
        // PlayerStats의 최신 상태를 currentSaveData에 반영
        currentSaveData.playerData = PlayerData.FromData(playerStats);
        currentSaveData.inventoryData = InventoryData.FromData(inventory);

        // currentSaveData를 JSON 형식으로 변환
        string json = JsonUtility.ToJson(currentSaveData, true);

        // 파일로 저장
        File.WriteAllText(savePath, json);
        Debug.Log("세이브 완료");
    }

    // 게임 데이터를 JSON에서 불러오는 함수
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            // JSON 파일을 읽어 currentSaveData로 로드
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);

            // PlayerStats에 로드된 데이터를 반영
            currentSaveData.playerData.ApplyToStats(playerStats);
            // InventoryManager에 로드된 데이터를 반영
            currentSaveData.inventoryData.ApplyToInventory(inventory);

            Debug.Log("로드 완료");
        }
        else
        {
            Debug.LogWarning("세이브를 찾을 수 없음");
        }
    }

    public void ResetAllData()
    {
        playerStats.healthLevel.Value = 0;
        playerStats.staminaLevel.Value = 0;
        playerStats.speedLevel.Value = 0;
        playerStats.inventoryLevel.Value = 0;
        playerStats.mapLevel.Value = 0;
        inventory.inventorySlotCount.Value = 20;

        // 저장된 파일도 삭제 (SaveManager가 save.json 등에 저장한다고 가정)
        DeleteSaveFile(); 

        // 새로 저장
        SaveGame();

        Debug.Log("세이브 파일 리셋 완료");
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("세이브 파일 삭제됨");
        }
        else
        {
            Debug.Log("삭제할 세이브 파일이 없음");
        }
    }
}