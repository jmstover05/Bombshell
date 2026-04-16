using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class settingsMenu : MonoBehaviour
{
    public Slider HorizontalMouseSensitivity;
    public Slider VerticalMouseSensitivity;
    public TextMeshProUGUI horizTMP;
    public TextMeshProUGUI vertTMP;

    private float horizSensitivityDefault = 20;
    private float vertSensitivityDefault = 8;

    private void Start()
    {
        updateSliders();
    }

    // Update is called once per frame
    void Update()
    {
        vertTMP.text = $"{VerticalMouseSensitivity.value}";
        horizTMP.text = $"{HorizontalMouseSensitivity.value}";
    }

    private void OnBecameVisible()
    {
        updateSliders();
    }
    //updates the sliders to their current, actual values
    public void updateSliders()
    {
        HorizontalMouseSensitivity.value = SettingsManager.horizSensitivity;
        VerticalMouseSensitivity.value = SettingsManager.vertSensitivity;
    }
    public void restoreDefaults()
    {
        HorizontalMouseSensitivity.value = horizSensitivityDefault;
        VerticalMouseSensitivity.value = vertSensitivityDefault;
    }

    public void closeAndSaveChanges()
    {
        PlayerPrefs.SetFloat("horizSensitivity", HorizontalMouseSensitivity.value);
        PlayerPrefs.SetFloat("vertSensitivity", VerticalMouseSensitivity.value);

        PlayerPrefs.Save();

        SettingsManager.ApplySettings();

        gameObject.SetActive(false);
    }
}
