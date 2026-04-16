using UnityEngine;

public static class SettingsManager
{
    public static float horizSensitivity = 0.0f;
    public static float vertSensitivity = 0.0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameLoad()
    {
        ApplySettings();
    }
    public static void ApplySettings()
    {
        horizSensitivity = PlayerPrefs.GetFloat("horizSensitivity");
        vertSensitivity = PlayerPrefs.GetFloat("vertSensitivity");
    }
}
