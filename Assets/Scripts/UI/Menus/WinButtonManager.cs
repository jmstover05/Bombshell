using UnityEngine;
using UnityEngine.SceneManagement;

public class WinButtonManager : MonoBehaviour
{
    public void OnReturnButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Win");
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single); // Assumes scenes are named Win and Title
    }
}
