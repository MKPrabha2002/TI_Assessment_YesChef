using UnityEngine;

public static class HighScoreManager
{
    private const string HighScoreKey = "YesChef_HighScore";

    /// <summary>
    /// Retrieves the saved high score. Defaults to 0 if none exists.
    /// </summary>
    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    /// <summary>
    /// Compares the final score to the saved high score.
    /// If it's strictly greater, saves the new score and returns true.
    /// </summary>
    public static bool CheckAndSaveHighScore(int finalScore)
    {
        int currentHighScore = LoadHighScore();

        if (finalScore > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, finalScore);
            PlayerPrefs.Save();
            Debug.Log($"[HighScoreManager] NEW HIGH SCORE SAVED! {finalScore} beat the old record of {currentHighScore}.");
            return true; // Record was broken
        }

        Debug.Log($"[HighScoreManager] High score remains {currentHighScore}. (You scored {finalScore})");
        return false; // No new record
    }
}
