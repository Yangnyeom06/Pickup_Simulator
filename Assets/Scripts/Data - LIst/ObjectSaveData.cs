using UnityEngine;

[System.Serializable]
public class ObjectSaveData
{
    public string uniqueID;
    public bool isActive;
    public SerializableTransform transformData;

    public ObjectSaveData(GameObject obj)
    {
        uniqueID = obj.GetComponent<Item>().uniqueID;
        isActive = obj.activeSelf;
        transformData = new SerializableTransform(obj.transform);
    }

    public void Apply()
    {
        foreach (var obj in GameObject.FindObjectsOfType<Item>())
        {
            if (obj.uniqueID == uniqueID)
            {
                obj.gameObject.SetActive(isActive);
                transformData.ApplyTo(obj.transform);
                break;
            }
        }
    }
}