using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }
    public int selectSlotId;

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

    public void LoadPlayScene(int slotId)
    {
        selectSlotId = slotId;
        SceneManager.LoadScene("TestScene1234");
    }
}