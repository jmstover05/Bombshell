using UnityEngine;

public static class BombshellScoreSave
{
    private const string Prefix = "Bombshell_";

    private static string HighScoreKey(string sceneName) => Prefix + sceneName + "_HighScore";
    private static string LastCompletedScoreKey(string sceneName) => Prefix + sceneName + "_LastCompletedScore";
    private static string WinsKey(string sceneName) => Prefix + sceneName + "_Wins";

    public static int LoadHighScore(string sceneName)
    {
        return PlayerPrefs.GetInt(HighScoreKey(sceneName), 0);
    }

    public static int LoadLastCompletedScore(string sceneName)
    {
        return PlayerPrefs.GetInt(LastCompletedScoreKey(sceneName), 0);
    }

    public static int LoadWins(string sceneName)
    {
        return PlayerPrefs.GetInt(WinsKey(sceneName), 0);
    }

    public static void SaveInProgressHighScore(string sceneName, int currentScore)
    {
        int best = Mathf.Max(currentScore, LoadHighScore(sceneName));
        PlayerPrefs.SetInt(HighScoreKey(sceneName), best);
        PlayerPrefs.Save();
    }

    public static void SaveCompletedRun(string sceneName, int finalScore)
    {
        int best = Mathf.Max(finalScore, LoadHighScore(sceneName));

        PlayerPrefs.SetInt(LastCompletedScoreKey(sceneName), finalScore);
        PlayerPrefs.SetInt(HighScoreKey(sceneName), best);
        PlayerPrefs.SetInt(WinsKey(sceneName), LoadWins(sceneName) + 1);
        PlayerPrefs.Save();
    }
}