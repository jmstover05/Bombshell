using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void OnReturnButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Scenes/Credits");
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single); // Assumes scenes are named Credits and Title
    }
}
