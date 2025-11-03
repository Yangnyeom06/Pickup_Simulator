using UnityEngine;

[System.Serializable]
public class SerialDialogueLine
{
    [TextArea(3, 5)]
    public string text;
}

[System.Serializable]
public class DialogueLine
{
    public string name;
    
    [TextArea(3, 5)]
    public string text;
}

