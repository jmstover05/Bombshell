using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    public void OnEasyMode()
    {
        PlayerPrefs.SetInt("healthPoints", 200);
        PlayerPrefs.SetInt("ammo", 175);
        PlayerPrefs.Save();
        OnStartButtonClicked();
    }
    public void OnMediumMode()
    {
        PlayerPrefs.SetInt("healthPoints", 100);
        PlayerPrefs.SetInt("ammo", 100);
        PlayerPrefs.Save();
        OnStartButtonClicked();
    }
    public void OnHardMode()
    {
        PlayerPrefs.SetInt("healthPoints", 50);
        PlayerPrefs.SetInt("ammo", 50);
        PlayerPrefs.Save();
        OnStartButtonClicked();
    }
    public void OnStartButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Title");
        SceneManager.LoadScene("Scenes/GameLevel", LoadSceneMode.Single); // Assumes scenes are named Title and GameLevel
    }

    public void OnCreditsButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Title");
        SceneManager.LoadScene("Scenes/Credits", LoadSceneMode.Single); // Assumes scenes are named Title and Credits
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
