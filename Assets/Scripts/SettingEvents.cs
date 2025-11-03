using System;

public static class SettingsEvents
{
    public static event Action<float> OnLookSensitivityChanged;

    public static void RaiseLookSensitivityChanged(float value)
        => OnLookSensitivityChanged?.Invoke(value);
}