using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Title");
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single); // Assumes scenes are named Title and Game
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
