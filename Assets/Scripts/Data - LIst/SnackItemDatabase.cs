using UnityEngine;
using System.Collections.Generic;

public class SnackItemDatabase : MonoBehaviour
{
    public static SnackItemDatabase Instance { get; private set; }

    [SerializeField] private List<SnackItemData> allItems = new();

    private Dictionary<string, SnackItemData> snackItemDict = new();

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

    public SnackItemData GetSnackItemDataById(string id)
    {
        if (snackItemDict.TryGetValue(id, out SnackItemData snack))
            return snack;
        return null;
    }
}
