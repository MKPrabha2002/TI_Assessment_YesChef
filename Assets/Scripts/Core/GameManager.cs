using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Match Settings")]
    public int CurrentScore { get; private set; }
    public float TimeRemaining { get; private set; } = 180f; // 3 minutes
    public bool IsGameActive { get; private set; }
    public bool IsNewHighScore { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private MenuManager menuManager;

    [Header("Player Reference")]
    [SerializeField] private GameObject playerReference;

    private void Awake()
    {
        // Standard Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Player starts completely locked out until the MenuManager initiates the game
        if (playerReference != null)
        {
            PlayerMovement pMovement = playerReference.GetComponent<PlayerMovement>();
            if (pMovement != null) pMovement.enabled = false;

            PlayerInteraction pInteraction = playerReference.GetComponent<PlayerInteraction>();
            if (pInteraction != null) pInteraction.enabled = false;
        }
    }

    private void Update()
    {
        if (IsGameActive)
        {
            TimeRemaining -= Time.deltaTime;

            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f; // Strict clamp
                IsGameActive = false;
                EndMatch();
            }
        }
    }

    /// <summary>
    /// Begins the match, resetting the score and timer.
    /// This will be hooked up to the Main Menu UI button.
    /// </summary>
    public void StartMatch()
    {
        CurrentScore = 0;
        TimeRemaining = 180f;
        IsGameActive = true;
        
        Debug.Log("[GameManager] Match Started! 3 Minutes on the clock.");
    }

    /// <summary>
    /// Called directly by the MenuManager when the "Start" button is clicked
    /// </summary>
    public void EnablePlayerControls()
    {
        if (playerReference != null)
        {
            PlayerMovement pMovement = playerReference.GetComponent<PlayerMovement>();
            if (pMovement != null) pMovement.enabled = true;

            PlayerInteraction pInteraction = playerReference.GetComponent<PlayerInteraction>();
            if (pInteraction != null) pInteraction.enabled = true;
            
            Debug.Log("[GameManager] Player controls unlocked.");
        }
    }

    /// <summary>
    /// Aggregates the global score. Handles negative numbers perfectly.
    /// </summary>
    public void AddScore(int points)
    {
        if (IsGameActive)
        {
            CurrentScore += points;
            Debug.Log($"[GameManager] Score Updated! Current Score: {CurrentScore}");
        }
    }

    /// <summary>
    /// Freezes the player and finalizes the game state.
    /// </summary>
    private void EndMatch()
    {
        // Immediately check and save high score
        IsNewHighScore = HighScoreManager.CheckAndSaveHighScore(CurrentScore);

        Debug.Log($"[GameManager] Match Over! Final Score: {CurrentScore}. New High Score: {IsNewHighScore}");

        if (playerReference != null)
        {
            // Disable movement and interaction scripts strictly
            PlayerMovement pMovement = playerReference.GetComponent<PlayerMovement>();
            if (pMovement != null) pMovement.enabled = false;

            PlayerInteraction pInteraction = playerReference.GetComponent<PlayerInteraction>();
            if (pInteraction != null) pInteraction.enabled = false;
            
            Debug.Log("[GameManager] Player controls locked.");
        }
        else
        {
            Debug.LogError("[GameManager] Player Reference is missing! Could not lock controls.");
        }

        // Trigger the post-game UI
        if (menuManager != null)
        {
            menuManager.ShowGameOver();
        }
    }
}
