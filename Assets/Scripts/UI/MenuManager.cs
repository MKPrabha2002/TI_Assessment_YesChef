using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over Data")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject newHighScoreText;

    private void Start()
    {
        // Enforce exact initial state
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (newHighScoreText != null) newHighScoreText.SetActive(false);
    }

    /// <summary>
    /// Triggered by the UI Start Button
    /// </summary>
    public void OnStartButtonClicked()
    {
        if (startPanel != null) startPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnablePlayerControls();
            GameManager.Instance.StartMatch();
        }
    }

    /// <summary>
    /// Triggered by the GameManager at the end of the match
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {GameManager.Instance.CurrentScore}";
            }

            if (newHighScoreText != null)
            {
                newHighScoreText.SetActive(GameManager.Instance.IsNewHighScore);
            }
        }
    }

    /// <summary>
    /// Triggered by the UI Restart Button
    /// </summary>
    public void OnRestartButtonClicked()
    {
        // Reloads the currently active scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
