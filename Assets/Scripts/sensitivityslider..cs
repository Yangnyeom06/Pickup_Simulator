using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Slider sensitivitySlider;
    public settingData settingdata;

    void Start()
    {
        sensitivitySlider.value = settingdata.lookSensitivity;
        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        settingdata.lookSensitivity = value;
    }
}