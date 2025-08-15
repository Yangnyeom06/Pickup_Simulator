using UnityEngine;
using System.Collections.Generic;

public class SnackDatabase : MonoBehaviour
{
    public static SnackDatabase Instance { get; private set; }

    [SerializeField] private List<SnackData> allItems = new();

    private Dictionary<string, SnackData> snackItemDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var snack in allItems)
        {
            if (!snackItemDict.ContainsKey(snack.itemID))
                snackItemDict.Add(snack.itemID, snack);
            else
                Debug.LogWarning($"중복된 snackitemID: {snack.itemID}");
        }
    }

    public SnackData GetSnackDataById(string id)
    {
        if (snackItemDict.TryGetValue(id, out SnackData snack))
            return snack;
        return null;
    }
}
