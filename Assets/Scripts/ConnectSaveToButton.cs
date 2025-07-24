using UnityEngine;

public class ConnectSaveToButton : MonoBehaviour
{
    public void OnClickSave()
    {
        SaveManager.Instance.SaveGame(SaveManager.Instance.slotId);
    }

    public void OnClickLoad()
    {
        SaveManager.Instance.LoadGame(SaveManager.Instance.slotId);
    }

    public void OnClickResetAllData()
    {
        SaveManager.Instance.ResetAllData(SaveManager.Instance.slotId);
    }
}