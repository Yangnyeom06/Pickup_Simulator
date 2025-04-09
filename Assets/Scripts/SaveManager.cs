using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public PlayerStats playerStats;

     private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        SaveData data = new SaveData
        {
           playerStats = new StatData
           {
                a = playerStats.a.Value,
                b = playerStats.b.Value,
                c = playerStats.c.Value,
                d = playerStats.d.Value,
                e = playerStats.e.Value,
           }
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("MySave", json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey("MySave")) return;

        string json = PlayerPrefs.GetString("MySave");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        playerStats.a.SetValueWithoutNotify(data.playerStats.a);
        playerStats.b.SetValueWithoutNotify(data.playerStats.b);
        playerStats.c.SetValueWithoutNotify(data.playerStats.c);
        playerStats.d.SetValueWithoutNotify(data.playerStats.d);
        playerStats.e.SetValueWithoutNotify(data.playerStats.e);
    }
}