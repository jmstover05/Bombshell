using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public GameObject thisMenu;
    public GameObject Settings;

    public static bool paused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if(!paused)
            {
                Pause();
            }
            else
            {
                if(!Settings.activeInHierarchy) //only resume if settings submenu is not open
                {
                    Resume();
                }
            }
        }
    }

    public void Pause()
    {
        thisMenu.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;

    }

    public void Resume()
    {
        thisMenu.SetActive(false);
        Time.timeScale = 1.0f;
        paused = false;
    }
    //opens the settings menu
    public void OpenSettings()
    {
        Settings.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
