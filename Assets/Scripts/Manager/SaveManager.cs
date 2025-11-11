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
        Debug.Log(Application.persistentDataPath + $"/gameSave_slot_{slotId}.json");
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



    private string GetSavePath(int slotId)
    {
        return Application.persistentDataPath + $"/gameSave_slot_{slotId}.json";
    }

    public void SaveGame(int slotId)
    {
        currentSaveData.playerData = PlayerData.FromData(PlayerManager.Instance);
        currentSaveData.inventoryData = InventoryData.FromData(InventoryManager.Instance);
        currentSaveData.gameData = GameDate.FromData(DayManager.Instance);
        currentSaveData.collectedItemsData = new List<ItemInstanceData>(ItemRepository.Instance.collectedItemDatas);


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

    public void LoadGame(int SlotId)
    {
        string savePath = GetSavePath(SlotId);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);

            currentSaveData.playerData.ApplyToPlayer(PlayerManager.Instance);
            currentSaveData.inventoryData.ApplyToInventory(InventoryManager.Instance);
            currentSaveData.gameData.ApplyToGame(DayManager.Instance);

            foreach (var uiData in currentSaveData.uiObjects)
                uiData.Apply();

            Debug.Log($"로드 완료 (슬롯 {SlotId})");

            if (ItemRepository.Instance != null)
            {
                ItemRepository.Instance.collectedItemDatas = new List<ItemInstanceData>(currentSaveData.collectedItemsData);
                ItemRepository.Instance.SpawnLoadedItems();
            }
        }
        else
        {
            Debug.LogWarning($"세이브 파일 없음: 슬롯 {SlotId}");
        }
    }

    public void newGame()
    {
        for (int i = 1; i < 3; i++)
        {
            if (!File.Exists(GetSavePath(i)))
            {
                slotId = i;
                Debug.Log($"{i}번 슬롯 로드");
                SceneManager.LoadScene("Map");

                break;
            }
            else
            {
                Debug.Log("빈 슬롯이 없습니다.");
            }
        }
    }

    public void continueGame()
    {
        for (int i = 1; i < 3; i++)
        {
            if (File.Exists(GetSavePath(i)))
            {
                slotId = i;
                Debug.Log($"{i}번 슬롯 로드");

                break;
            }
            else
            {
                Debug.Log("세이브 슬롯이 없습니다.");
            }
        }
    }


    public void ResetAllData(int slotId)
    {
        Debug.Log(slotId);
        PlayerManager.Instance.ResetPlayerData();
        InventoryManager.Instance.ResetSlots();
        DayManager.Instance.ResetDayData();
        ItemRepository.Instance.ResetCollectedItems();

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
