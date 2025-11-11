using UnityEngine;

public class SaveLoadTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Debug.Log(SaveManager.Instance.slotId);
        SaveManager.Instance.LoadGame(SaveManager.Instance.slotId);
        CursorControl.Instance.CursorCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
