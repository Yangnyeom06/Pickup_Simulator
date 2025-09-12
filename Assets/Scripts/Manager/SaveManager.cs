using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData currentSaveData = new();

    public int slotId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private string GetSavePath(int slotId)
    {
        return Application.persistentDataPath + $"/gameSave_slot_{slotId}.json";
    }

    public void SaveGame(int slotId)
    {
        currentSaveData.playerData = PlayerData.FromData(PlayerManager.Instance);
        currentSaveData.inventoryData = InventoryData.FromData(InventoryManager.Instance);
        currentSaveData.gameData = GameDate.FromData(DayManager.Instance);

        // 오브젝트와 UI 저장
        currentSaveData.uiObjects.Clear();

        foreach (var u in Object.FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!u.gameObject.scene.IsValid()) continue;

            GameObject go = u.gameObject;

            if (go.GetComponent<UnityEngine.UI.Text>() || go.GetComponent<TMPro.TMP_Text>() || go.GetComponent<UnityEngine.UI.Slider>() || go.GetComponent<UnityEngine.UI.Image>())
            {
                currentSaveData.uiObjects.Add(new UISaveData(go));
            }
        }

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(GetSavePath(slotId), json);
        Debug.Log($"세이브 완료 (슬롯 {slotId})");
    }

    public void LoadGame(int slotId)
    {
        string savePath = GetSavePath(slotId);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);

            currentSaveData.playerData.ApplyToPlayer(PlayerManager.Instance);
            currentSaveData.inventoryData.ApplyToInventory(InventoryManager.Instance);
            currentSaveData.gameData.ApplyToGame(DayManager.Instance);

            foreach (var uiData in currentSaveData.uiObjects)
                uiData.Apply();

            Debug.Log($"로드 완료 (슬롯 {slotId})");
        }
        else
        {
            Debug.LogWarning($"세이브 파일 없음: 슬롯 {slotId}");
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene1234")
        {
            if (PlayerManager.Instance != null && InventoryManager.Instance != null)
            {
                slotId = SceneChangeManager.Instance.selectSlotId;
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
        PlayerManager.Instance.ResetPlayerData();
        InventoryManager.Instance.ResetSlots();
        DayManager.Instance.ResetDayData();

        DeleteSaveFile(slotId);
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
