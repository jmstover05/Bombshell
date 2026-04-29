using UnityEngine;

public class SelfDestructManager: MonoBehaviour
{
    public SelfDestructHudManager HUD;
    [Tooltip("The number of seconds the self-destruct timer starts with")]
    public float initialTimeLeft = 120.0f;

    public float currentTime = 0.0f;
    public bool isSelfDestructing = false;
    public bool isDestroyed = false;

    public Light light;

    private void Start()
    {
        currentTime = initialTimeLeft;
    }

    private void Update()
    {
        if (isSelfDestructing)
        {
            light.color = Color.red;
            if (currentTime > 0.0f)
            {
                currentTime -= Time.deltaTime;
                int minutes = Mathf.FloorToInt(currentTime) / 60;
                int seconds = Mathf.FloorToInt(currentTime) % 60;
                int miliseconds = Mathf.FloorToInt(currentTime * 100) % 100;
                HUD.Timer.text = $"{minutes:00}:{seconds:00}:{miliseconds:00}";
            }
            else
            {
                SelfDestruct();
            }
        }
    }
    public float GetTimeLeft()
    { 
        return currentTime; 
    }
    public bool IsSelfDestructing()
    {
        return isSelfDestructing;
    }
    /// <summary>
    /// Starts the self-distruct timer
    /// Should be called when the player reaches the "end" of the level
    /// </summary>
    public void StartSelfDestructTimer()
    {
        isSelfDestructing = true;
        HUD.background.SetActive(true);
    }
    /// <summary>
    /// Stops the self-destruct timer
    /// Should be called when the player returns to the beginning of the level after starting the self-destruct sequence
    /// </summary>
    public void StopSelfDestructTimer()
    {
        isSelfDestructing = false;
        //play ending animation instead???
        HUD.background.SetActive(false);
    }
    /// <summary>
    /// Should end the game as a loss for the player by either setting the player's health to zero, or switching to
    /// a unique scene that says the player failed to escape in time.
    /// </summary>
    private void SelfDestruct()
    {
        HUD.Timer.text = "00:00:00";
        Debug.Log("AHH YOU EXPLODED");
        isDestroyed = true;
        return;
    }

}
