using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BombshellGameManager : MonoBehaviour
{
    public static BombshellGameManager Instance;

    [Header("References")]
    public PlayerMovement player;
    public Transform finalEscapeCheckpoint;
    public EscapeTrigger escapeTrigger;
    public GoalTrigger goalTrigger;

    [Header("Rules")]
    public float selfDestructDuration = 30.0f;
    public int enemyKillScore = 100;
    public int goalBonusScore = 500;
    public int deathPenalty = 50;

    [Header("Messages")]
    public float shortMessageDuration = 2.0f;

    private Vector3 checkpointPosition;
    private int score;
    private int highScore;
    private float timerRemaining;
    private bool selfDestructActive;
    private bool levelComplete;
    private bool checkpointsLocked;

    private string statusMessage = "";
    private string endGameMessage = "";
    private float statusMessageTimer = 0.0f;

    private bool waitingForContinue = false;
    private string continueMessage = "";
    private bool pendingRespawn = false;

    private GUIStyle hudStyle;
    private GUIStyle statusStyle;
    private GUIStyle endStyle;
    private GUIStyle panelStyle;
    private GUIStyle buttonStyle;

    private LevelSnapshot currentSnapshot;

    [System.Serializable]
    private class EnemySnapshot
    {
        public BombshellEnemy enemy;
        public bool active;
        public Vector3 position;
        public Quaternion rotation;
        public float health;
    }

    [System.Serializable]
    private class LevelSnapshot
    {
        public Vector3 checkpointPosition;
        public int score;
        public float timerRemaining;
        public bool selfDestructActive;
        public bool checkpointsLocked;
        public bool escapeUsed;
        public bool goalUsed;
        public List<EnemySnapshot> enemies = new List<EnemySnapshot>();
    }

    public Transform PlayerTransform => player != null ? player.transform : null;
    public bool SelfDestructActive => selfDestructActive;
    public bool LevelComplete => levelComplete;
    public bool CheckpointsLocked => checkpointsLocked;
    public bool WaitingForContinue => waitingForContinue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
        }

        if (escapeTrigger == null)
        {
            escapeTrigger = FindAnyObjectByType<EscapeTrigger>();
        }

        if (goalTrigger == null)
        {
            goalTrigger = FindAnyObjectByType<GoalTrigger>();
        }

        if (player != null)
        {
            checkpointPosition = player.transform.position;
        }

        checkpointsLocked = false;
        selfDestructActive = false;
        levelComplete = false;
        waitingForContinue = false;
        pendingRespawn = false;
        endGameMessage = "";

        string key = SceneManager.GetActiveScene().name;
        score = 0;
        highScore = PlayerPrefs.GetInt(key + "_HighScore", 0);

        SetupGuiStyles();
        statusMessage = "Reach the escape trigger.";
        statusMessageTimer = 0.0f;

        CaptureLevelSnapshot();
    }

    void Update()
    {
        if (levelComplete || waitingForContinue)
        {
            return;
        }

        if (selfDestructActive)
        {
            timerRemaining -= Time.deltaTime;

            if (timerRemaining <= 0.0f)
            {
                timerRemaining = 0.0f;
                LoseToTimeout("Time ran out.");
                return;
            }
        }

        if (statusMessageTimer > 0.0f)
        {
            statusMessageTimer -= Time.deltaTime;

            if (statusMessageTimer <= 0.0f)
            {
                statusMessageTimer = 0.0f;
                statusMessage = GetDefaultObjectiveText();
            }
        }
        else
        {
            statusMessage = GetDefaultObjectiveText();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score < 0)
        {
            score = 0;
        }

        if (score > highScore)
        {
            highScore = score;
        }

        SaveData();
    }

    public void AddEnemyKillScore()
    {
        AddScore(enemyKillScore);
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        if (checkpointsLocked)
        {
            return;
        }

        checkpointPosition = newCheckpoint;
        SaveData();
        SetTemporaryStatus("Checkpoint reached.", shortMessageDuration);
        CaptureLevelSnapshot();
    }

    public void StartSelfDestruct()
    {
        if (selfDestructActive || levelComplete)
        {
            return;
        }

        selfDestructActive = true;
        checkpointsLocked = true;
        timerRemaining = selfDestructDuration;

        if (finalEscapeCheckpoint != null)
        {
            checkpointPosition = finalEscapeCheckpoint.position;
        }

        RemoveAllEnemies();
        SaveData();

        statusMessage = "SELF DESTRUCTION IN: " + timerRemaining.ToString("F1");
        statusMessageTimer = 0.0f;

        CaptureLevelSnapshot();
    }

    public void CompleteLevel()
    {
        if (!selfDestructActive || levelComplete)
        {
            return;
        }

        selfDestructActive = false;
        levelComplete = true;
        waitingForContinue = false;
        pendingRespawn = false;

        AddScore(goalBonusScore);

        string key = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(key + "_Wins", PlayerPrefs.GetInt(key + "_Wins", 0) + 1);
        PlayerPrefs.Save();

        endGameMessage =
            "GOAL REACHED!\n\n" +
            "YOU ESCAPED!\n\n" +
            "FINAL SCORE: " + score + "\n" +
            "HIGH SCORE: " + highScore;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoseAndRespawn(string message)
    {
        RestoreCheckpointSnapshot(deathPenalty, message, false);
    }

    public void LoseToEnemy(string message)
    {
        BeginContinueDeath(message);
    }

    public void LoseToHazard(string message)
    {
        BeginContinueDeath(message);
    }

    public void LoseToTimeout(string message)
    {
        BeginContinueDeath(message);
    }

    private void BeginContinueDeath(string message)
    {
        if (levelComplete || waitingForContinue)
        {
            return;
        }

        selfDestructActive = false;
        timerRemaining = 0.0f;

        waitingForContinue = true;
        pendingRespawn = true;
        continueMessage = message;
        statusMessage = message;
        statusMessageTimer = 0.0f;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueAfterDeathScreen()
    {
        if (!pendingRespawn)
        {
            return;
        }

        RestoreCheckpointSnapshot(deathPenalty, null, true);
    }

    private void RestoreCheckpointSnapshot(int scorePenalty, string tempMessage, bool fromContinue)
    {
        if (currentSnapshot == null)
        {
            if (player != null)
            {
                player.TeleportTo(checkpointPosition);
            }

            waitingForContinue = false;
            pendingRespawn = false;
            continueMessage = "";
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (!string.IsNullOrEmpty(tempMessage))
            {
                SetTemporaryStatus(tempMessage, shortMessageDuration);
            }

            return;
        }

        selfDestructActive = currentSnapshot.selfDestructActive;
        checkpointsLocked = currentSnapshot.checkpointsLocked;
        levelComplete = false;
        waitingForContinue = false;
        pendingRespawn = false;
        continueMessage = "";
        endGameMessage = "";

        checkpointPosition = currentSnapshot.checkpointPosition;
        timerRemaining = currentSnapshot.timerRemaining;
        score = Mathf.Max(0, currentSnapshot.score - scorePenalty);

        if (score > highScore)
        {
            highScore = score;
        }

        foreach (EnemySnapshot enemyState in currentSnapshot.enemies)
        {
            if (enemyState.enemy != null)
            {
                enemyState.enemy.ApplySnapshot(
                    enemyState.active,
                    enemyState.position,
                    enemyState.rotation,
                    enemyState.health
                );
            }
        }

        if (escapeTrigger != null)
        {
            escapeTrigger.SetUsed(currentSnapshot.escapeUsed);
        }

        if (goalTrigger != null)
        {
            goalTrigger.SetUsed(currentSnapshot.goalUsed);
        }

        if (player != null)
        {
            player.TeleportTo(checkpointPosition);
        }

        SaveData();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!string.IsNullOrEmpty(tempMessage))
        {
            SetTemporaryStatus(tempMessage, shortMessageDuration);
        }
        else if (fromContinue)
        {
            statusMessage = GetDefaultObjectiveText();
            statusMessageTimer = 0.0f;
        }
    }

    private void CaptureLevelSnapshot()
    {
        currentSnapshot = new LevelSnapshot();
        currentSnapshot.checkpointPosition = checkpointPosition;
        currentSnapshot.score = score;
        currentSnapshot.timerRemaining = timerRemaining;
        currentSnapshot.selfDestructActive = selfDestructActive;
        currentSnapshot.checkpointsLocked = checkpointsLocked;
        currentSnapshot.escapeUsed = escapeTrigger != null && escapeTrigger.Used;
        currentSnapshot.goalUsed = goalTrigger != null && goalTrigger.Used;

        foreach (BombshellEnemy enemy in BombshellEnemy.Registry)
        {
            if (enemy == null)
            {
                continue;
            }

            EnemySnapshot enemyState = new EnemySnapshot();
            enemyState.enemy = enemy;
            enemyState.active = enemy.gameObject.activeSelf;
            enemyState.position = enemy.transform.position;
            enemyState.rotation = enemy.transform.rotation;
            enemyState.health = enemy.CurrentHealth;

            currentSnapshot.enemies.Add(enemyState);
        }
    }

    private void RemoveAllEnemies()
    {
        foreach (BombshellEnemy enemy in BombshellEnemy.Registry)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }

    private void SaveData()
    {
        string key = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(key + "_LastScore", score);
        PlayerPrefs.SetInt(key + "_HighScore", highScore);
        PlayerPrefs.Save();
    }

    private string GetDefaultObjectiveText()
    {
        if (selfDestructActive)
        {
            return "SELF DESTRUCTION IN: " + timerRemaining.ToString("F1");
        }

        return "Reach the escape trigger.";
    }

    private void SetTemporaryStatus(string message, float duration)
    {
        statusMessage = message;
        statusMessageTimer = duration;
    }

    private void SetupGuiStyles()
    {
        hudStyle = new GUIStyle();
        hudStyle.fontSize = 22;
        hudStyle.normal.textColor = Color.white;

        statusStyle = new GUIStyle();
        statusStyle.fontSize = 26;
        statusStyle.alignment = TextAnchor.UpperCenter;
        statusStyle.normal.textColor = Color.red;

        endStyle = new GUIStyle();
        endStyle.fontSize = 34;
        endStyle.alignment = TextAnchor.MiddleCenter;
        endStyle.normal.textColor = Color.white;

        panelStyle = new GUIStyle();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.85f));
        tex.Apply();
        panelStyle.normal.background = tex;
    }

    void OnGUI()
    {
        if (hudStyle == null)
        {
            SetupGuiStyles();
        }

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 22;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(20, 20, 250, 40), "Score: " + score, hudStyle);
        GUI.Label(new Rect(20, 50, 250, 40), "High Score: " + highScore, hudStyle);

        if (!levelComplete)
        {
            GUI.Label(
                new Rect(Screen.width / 2 - 260, 20, 520, 40),
                statusMessage,
                statusStyle
            );
        }

        if (waitingForContinue)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", panelStyle);

            GUI.Label(
                new Rect(Screen.width / 2 - 250, Screen.height / 2 - 140, 500, 120),
                continueMessage,
                endStyle
            );

            if (GUI.Button(
                new Rect(Screen.width / 2 - 100, Screen.height / 2 + 10, 200, 60),
                "Continue",
                buttonStyle))
            {
                ContinueAfterDeathScreen();
            }
        }

        if (levelComplete)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", panelStyle);

            GUI.Label(
                new Rect(Screen.width / 2 - 250, Screen.height / 2 - 140, 500, 280),
                endGameMessage,
                endStyle
            );
        }
    }
}