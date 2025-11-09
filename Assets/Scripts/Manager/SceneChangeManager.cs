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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
<<<<<<< HEAD
        // Slum ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Îµï¿½ï¿½ ï¿½ï¿½, Player ï¿½ï¿½Ä¡ ï¿½Ìµï¿½
=======
        // Slum °°Àº ¾ÀÀÌ ·ÎµåµÈ ÈÄ, Player À§Ä¡ ÀÌµ¿
>>>>>>> 74b4bcf0 (update)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && !string.IsNullOrEmpty(nextSpawnPointName))
        {
            GameObject spawn = GameObject.Find(nextSpawnPointName);
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
            }
        }
    }
}
