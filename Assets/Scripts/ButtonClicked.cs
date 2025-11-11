using UnityEngine;

public class ButtonClicked : MonoBehaviour
{
    public void OnClickedSave()
    {
        Debug.Log("버튼 클릭됨");
        FadeInOut.Instance.StartFadeInOut();
        SaveManager.Instance.SaveGame(SaveManager.Instance.slotId);
        Debug.Log($"{SaveManager.Instance.slotId}번 슬롯 저장됨");
    }
}
