using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI pauseButtonText;

    // Caching state to avoid unnecessary UI string allocations every frame
    private int lastKnownTime = -1;
    private int lastKnownScore = -1;

    private void Start()
    {
        // Load and immediately display the high score
        int loadedHighScore = HighScoreManager.LoadHighScore();
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {loadedHighScore}";
        }
    }

    private void Update()
    {
        // Ensure GameManager exists before attempting to read from it
        if (GameManager.Instance == null) return;

        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    /// <summary>
    /// Reads the global timer. Only updates the string allocation if the whole second has changed.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int currentWholeSecond = Mathf.FloorToInt(GameManager.Instance.TimeRemaining);

        // Optimization: Only format the string if the second has actually ticked down
        if (currentWholeSecond != lastKnownTime)
        {
            lastKnownTime = currentWholeSecond;

            int minutes = currentWholeSecond / 60;
            int seconds = currentWholeSecond % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    /// <summary>
    /// Reads the global score. Only updates the string allocation if the integer has changed.
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (currentScoreText == null) return;

        int currentScore = GameManager.Instance.CurrentScore;

        // Optimization: Only convert to string if the score actually changed
        if (currentScore != lastKnownScore)
        {
            lastKnownScore = currentScore;
            currentScoreText.text = $"Score: {currentScore}";
        }
    }

    /// <summary>
    /// Toggles the Time.timeScale and updates the pause button text.
    /// </summary>
    public void TogglePause()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
            if (pauseButtonText != null) pauseButtonText.text = "Resume";
            Debug.Log("[HUDManager] Game Paused");
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseButtonText != null) pauseButtonText.text = "Pause";
            Debug.Log("[HUDManager] Game Resumed");
        }
    }

    /// <summary>
    /// Safely shuts down the application in both Editor and Builds.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[HUDManager] Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
