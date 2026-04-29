using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseButtonManager : MonoBehaviour
{
    public void OnReturnButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Lose");
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single); // Assumes scenes are named Lose and Title
    }
}
