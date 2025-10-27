using UnityEngine;

public class ResolutionDebugGUI : MonoBehaviour
{
     void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 800, 25),
            $"Screen: {Screen.width}x{Screen.height}  Mode: {Screen.fullScreenMode}");
    }
}