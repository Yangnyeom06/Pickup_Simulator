using UnityEngine;

[System.Serializable]
public class UISaveData
{
    public string uniqueID;
    public bool isActive;
    public string componentType;
    public string stringValue;  // Text or color(string)
    public float floatValue;

    public SerializableTransform transformData;

    public UISaveData(GameObject obj)
    {
        uniqueID = obj.GetComponent<Item>().uniqueID;
        isActive = obj.activeSelf;

        if (obj.TryGetComponent(out TMPro.TMP_Text tmp))
        {
            componentType = "TMP";
            stringValue = tmp.text;
        }
        else if (obj.TryGetComponent(out UnityEngine.UI.Text text))
        {
            componentType = "Text";
            stringValue = text.text;
        }
        else if (obj.TryGetComponent(out UnityEngine.UI.Slider slider))
        {
            componentType = "Slider";
            floatValue = slider.value;
        }
        else if (obj.TryGetComponent(out UnityEngine.UI.Image image))
        {
            componentType = "Image";
            stringValue = ColorUtility.ToHtmlStringRGBA(image.color);
        }

        transformData = new SerializableTransform(obj.transform);
    }

    public void Apply()
    {
        foreach (var obj in Object.FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            if (obj.uniqueID == uniqueID)
            {
                obj.gameObject.SetActive(isActive);
                transformData.ApplyTo(obj.transform);

                switch (componentType)
                {
                    case "TMP":
                        if (obj.TryGetComponent(out TMPro.TMP_Text tmp))
                            tmp.text = stringValue;
                        break;
                    case "Text":
                        if (obj.TryGetComponent(out UnityEngine.UI.Text text))
                            text.text = stringValue;
                        break;
                    case "Slider":
                        if (obj.TryGetComponent(out UnityEngine.UI.Slider slider))
                            slider.value = floatValue;
                        break;
                    case "Image":
                        if (obj.TryGetComponent(out UnityEngine.UI.Image image) &&
                            ColorUtility.TryParseHtmlString("#" + stringValue, out Color color))
                        {
                            image.color = color;
                        }
                        break;
                }

                break;
            }
        }
    }
}