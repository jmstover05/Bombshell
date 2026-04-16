using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    void OnStartButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Title");
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single); // Assumes scenes are named Title and Game
    }

    void OnCreditsButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Title");
        SceneManager.LoadScene("Scenes/Credits", LoadSceneMode.Single); // Assumes scenes are named Title and Credits
    }
}
