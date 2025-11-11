using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class SaveSlotText : MonoBehaviour
{
    public int slotId;
    public SaveData currentSaveData = new();
    public TextMeshProUGUI saveText;
    private string GetSavePath(int slotId)
    {
        return Application.persistentDataPath + $"/gameSave_slot_{slotId}.json";
    }
    private void Awake()
    {
        string savePath = GetSavePath(slotId);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);
            saveText.text = $"{currentSaveData.gameData.year}년 {currentSaveData.gameData.month}월 {currentSaveData.gameData.day}일\n";
            saveText.text += $"{currentSaveData.playerData.money}원\n";
        }
        else
        {
            saveText.text = "Empty";
        }
    }
}
