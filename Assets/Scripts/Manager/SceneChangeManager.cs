using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }
    public SaveManager saveManager;
    public int selectSlotId;

    // add
    public string nextSpawnPointName;

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

    // modify
    public void LoadPlayScene(string sceneName, int slotId, string spawnPointName = "SpawnPoint")
    {
        selectSlotId = slotId;
        nextSpawnPointName = spawnPointName;

        SceneManager.LoadScene(sceneName);
    }
}
