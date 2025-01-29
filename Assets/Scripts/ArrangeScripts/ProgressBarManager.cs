using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the progress bar functionality in the game.
/// Updates the progress bar UI Image as progress increases
/// and notifies the GameLevelManager when the level is complete.
/// </summary>
public class ProgressBarManager : MonoBehaviour
{
    // [SerializeField] private GameDataManager gameDataManager; // Reference to GameDataManager via the Inspector.

    [Header("Progress Tracking")]
    [Tooltip("Tracks the current progress based on the number of items placed in the correct slots.")]
    [SerializeField] private int currentProgress;

    [Tooltip("The total number of items required to complete the level.")]
    [SerializeField] private int totalItems;

    [Header("Progress Bar UI")]
    [Tooltip("Array of sprites representing different progress stages.")]
    public Sprite[] progressBarStages;

    [Tooltip("The UI Image component that displays the current progress.")]
    public Image progressBarImage;

    [Header("Audio Settings")]
    [Tooltip("Sound played when progress is updated.")]
    public AudioClip progressSound;
    private AudioSource audioSource;
    private SceneManagement sceneManagement;
    private int maxScore = 100;

    [Header("Scriptable Objects")]
    [SerializeField] private ScoreData scoreData; // Reference to ScoreData.
    [SerializeField] private TimeData timeData; // Reference to TimeData.
    [SerializeField] private GameStats gameStats; // Reference to GameStats.

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    private float startTime;

    private void Start()
    {
        // Check if the Scriptable Objects are properly assigned
        if (scoreData == null)
        {
            Debug.LogError("[ProgressBarManager] ScoreData is not assigned in the inspector!");
        }
        if (timeData == null)
        {
            Debug.LogError("[ProgressBarManager] TimeData is not assigned in the inspector!");
        }
        if (gameStats == null)
        {
            Debug.LogError("[ProgressBarManager] GameStats is not assigned in the inspector!");
        }

        // Add AudioSource if not already attached
        audioSource = gameObject.AddComponent<AudioSource>();
        UpdateProgressBar();

        startTime = Time.time; // Start measuring time
        var sceneManagement = SceneManagement.Instance.GetComponent<SceneManagement>();
        if (sceneManagement != null)
        {

            Debug.Log($"[ProgressBarManager] Successfully accessed SceneManagement: {sceneManagement}");
        }
        else
        {
            Debug.LogError("[ProgressBarManager] SceneManagement is not available in StagesTrack!");
        }
    }

    /// <summary>
    /// Initializes the progress bar with the total number of items required for the level.
    /// </summary>
    /// <param name="itemsCount">Total items required for level completion.</param>
    public void InitializeProgressBar(int itemsCount)
    {
        //totalItems = itemsCount;
        Debug.Log($"Initializing Progress Bar with {itemsCount} items.");
        UpdateProgressBar();
    }

    /// <summary>
    /// Adds progress when an item is successfully placed in the correct slot.
    /// </summary>
    public void AddProgress()
    {
        if (currentProgress < totalItems)
        {
            currentProgress++;
            UpdateProgressBar();

            if (progressSound != null && currentProgress < totalItems)
            {
                audioSource.PlayOneShot(progressSound);
            }

            if (currentProgress == totalItems)
            {
                OnStageComplete();
            }
        }
    }

    /// <summary>
    /// Handles logic when the stage is complete.
    /// </summary>
    private void OnStageComplete()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        var sceneManagement = SceneManagement.Instance.GetComponent<SceneManagement>();
        if (sceneManagement != null)
        {

            Debug.Log($"[ProgressBarManager] Successfully accessed SceneManagement: {sceneManagement}");
        }
        else
        {
            Debug.LogError("[ProgressBarManager] SceneManagement is not available in StagesTrack!");
        }
        string gameName = sceneManagement.GetCurrentGameName();

        float timeTaken = Time.time - startTime;

        // Calculate score based on time taken (higher score for faster completion)
        int calculatedScore = CalculateScore(timeTaken);

        // Update Scriptable Objects
        scoreData.UpdateScore(gameName, currentSceneName, calculatedScore);
        timeData.UpdateTime(gameName, currentSceneName, timeTaken);
        gameStats.UpdateStageStats(gameName, currentSceneName, calculatedScore, timeTaken);

        // Display results
        DisplayResults(gameName, timeTaken, calculatedScore);

        // Notify scene transition by showing the Level-Up screen
        var levelUpManager = Object.FindFirstObjectByType<LevelUpManager>();
        if (levelUpManager != null)
        {
            levelUpManager.ShowLevelUpScreen();
        }
        else
        {
            Debug.LogError("[ExitTrigger] LevelUpManager not found in the scene.");
        }
    }

    /// <summary>
    /// Calculates the player's score based on the time taken to complete the stage.
    /// The faster the completion, the higher the score, with a gentle decrease for longer durations.
    /// </summary>
    /// <param name="timeTaken">The total time taken by the player to complete the stage.</param>
    /// <returns>An integer representing the player's score based on completion time.</returns>
    private int CalculateScore(float timeTaken)
    {
        // Constants for time thresholds
        const float FAST_COMPLETION_TIME = 60f;  // 1 minute
        const float MEDIUM_COMPLETION_TIME = 180f; // 3 minutes
        const float MAX_TIME_ALLOWED = 300f; // 5 minutes

        // Score boundaries
        const int MAX_SCORE = 100;
        const int HIGH_SCORE = 90;
        const int MEDIUM_SCORE = 75;
        const int MIN_SCORE = 65;

        if (timeTaken <= FAST_COMPLETION_TIME) // Within a minute 
        {
            // score between 100-90
            return Mathf.RoundToInt(HIGH_SCORE + ((FAST_COMPLETION_TIME - timeTaken) / FAST_COMPLETION_TIME) * (MAX_SCORE - HIGH_SCORE));
        }
        else if (timeTaken <= MEDIUM_COMPLETION_TIME) // Between 1 min to 3 min
        {
            // Score between 90-75 decrease according to time
            float normalizedTime = (timeTaken - FAST_COMPLETION_TIME) / (MEDIUM_COMPLETION_TIME - FAST_COMPLETION_TIME);
            return Mathf.RoundToInt(MEDIUM_SCORE + (HIGH_SCORE - MEDIUM_SCORE) * (1 - normalizedTime));
        }
        else // More than 3 min
        {
            // Score between 75-65
            float normalizedTime = Mathf.Clamp01((timeTaken - MEDIUM_COMPLETION_TIME) / (MAX_TIME_ALLOWED - MEDIUM_COMPLETION_TIME));
            return Mathf.RoundToInt(MIN_SCORE + (MEDIUM_SCORE - MIN_SCORE) * (1 - normalizedTime));
        }
    }

    /// <summary>
    /// Displays results for the completed stage.
    /// </summary>
    private void DisplayResults(string gameName, float timeTaken, int score)
    {
        timeText.text = $"זמן: {timeTaken:F2} שניות";
        scoreText.text = $"ניקוד: {score}";
        Debug.Log($"[ProgressBarManager] Game: {gameName}, Time: {timeTaken:F2}, Score: {score}");
    }

    /// <summary>
    /// Updates the progress bar sprite to reflect the current progress.
    /// </summary>
    private void UpdateProgressBar()
    {
        if (progressBarStages.Length > 0 && progressBarImage != null)
        {
            int index = Mathf.Clamp(currentProgress, 0, progressBarStages.Length - 1);
            progressBarImage.sprite = progressBarStages[index];
        }
        else
        {
            Debug.LogWarning("Progress bar images are not properly set up.");
        }
    }
}
