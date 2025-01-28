using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeScore : MonoBehaviour
{
    [Header("Score Settings")]
    public int maxScore = 100; // Maximum possible score per level
    [SerializeField] private float maxTimeForScore = 600f; // Maximum time in seconds for scoring (default 10 minutes)


    [Header("Scriptable Objects")]
    [SerializeField] private ScoreData scoreData; // Reference to ScoreData
    [SerializeField] private TimeData timeData; // Reference to TimeData
    [SerializeField] private MiniGamesAndStages miniGamesAndStages; // Reference to MiniGamesAndStages
    [SerializeField] private GameStats gameStats; // Reference to GameStats


    [Header("UI Elements")]
    public TMP_Text scoreText; // Display for the current score
    public TMP_Text timeText; // Display for the current time

    private string currentSceneName;
    private float startTime;
    private int currentScore;


    //consts
    private const int ZERO = 0;
    private const float ONE = 1;



    private void Awake()
    {
        InitializeReferences();
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        StartLevel();
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != currentSceneName)
        {
            Debug.Log($"[ScoreManagerr] Scene changed from {currentSceneName} to {sceneName}");
            currentSceneName = sceneName;
            StartLevel();
        }
    }

    /// <summary>
    /// Initializes the scriptable objects and UI references.
    /// </summary>
    private void InitializeReferences()
    {
        if (scoreData == null)
        {
            scoreData = Object.FindFirstObjectByType<ScoreData>();
            if (scoreData == null)
            {
                Debug.LogError("[ScoreManagerr] ScoreData not found. Ensure it exists in the scene.");
            }
        }

        if (timeData == null)
        {
            timeData = Object.FindFirstObjectByType<TimeData>();
            if (timeData == null)
            {
                Debug.LogError("[ScoreManagerr] TimeData not found. Ensure it exists in the scene.");
            }
        }

        if (miniGamesAndStages == null)
        {
            miniGamesAndStages = Object.FindFirstObjectByType<MiniGamesAndStages>();
            if (miniGamesAndStages == null)
            {
                Debug.LogError("[ScoreManagerr] MiniGamesAndStages not found. Ensure it exists in the scene.");
            }
        }

        InitializeUIElements();
    }

    /// <summary>
    /// Initializes or reassigns the UI elements.
    /// </summary>
    public void InitializeUIElements()
    {
        if (scoreText == null)
        {
            scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TMP_Text>();
            if (scoreText == null)
            {
                Debug.LogError("[ScoreManagerr] ScoreText not found or assigned. Ensure a GameObject with tag 'ScoreText' exists in the scene.");
            }
        }

        if (timeText == null)
        {
            timeText = GameObject.FindWithTag("TimeText")?.GetComponent<TMP_Text>();
            if (timeText == null)
            {
                Debug.LogError("[ScoreManagerr] TimeText not found or assigned. Ensure a GameObject with tag 'TimeText' exists in the scene.");
            }
        }
    }

    /// <summary>
    /// Starts tracking the level at the beginning of the scene.
    /// </summary>
    private void StartLevel()
    {
        startTime = Time.time;
        currentScore = maxScore;

        Debug.Log($"[ScoreManagerr] Level started for scene: {currentSceneName}");
    }

    /// <summary>
    /// Ends the current level and updates the Scriptable Objects with the results.
    /// </summary>
    public void EndLevel()
    {
        float timeTaken = Time.time - startTime;


        // Adjust score calculation based on maxTimeForScore
        if (timeTaken <= maxTimeForScore)
        {
            currentScore = Mathf.Max(ZERO, Mathf.RoundToInt(maxScore * Mathf.Max(ZERO, ONE - (timeTaken / maxTimeForScore))));
        }
        else
        {
            currentScore = ZERO; // If time exceeds maxTimeForScore, score is zero
        }




        string gameName = GetCurrentGameName();
        currentSceneName = SceneManager.GetActiveScene().name;
        if (gameName != null)
        {
            scoreData.UpdateScore(gameName, currentSceneName, currentScore);
            timeData.UpdateTime(gameName, currentSceneName, timeTaken);
            gameStats.UpdateStageStats(gameName, currentSceneName, currentScore, timeTaken);

            Debug.Log($"[ScoreManagerr] Updated ScoreData and TimeData for game: {gameName}, scene: {currentSceneName}, score: {currentScore}, time: {timeTaken:F2}");
        }
        else
        {
            Debug.LogError("[ScoreManagerr] Could not determine game name for the current scene.");
        }

        UpdateUIElements(currentScore, timeTaken);
    }

    /// <summary>
    /// Updates the score and time UI elements.
    /// </summary>
    private void UpdateUIElements(int score, float timeTaken)
    {
        if (scoreText != null)
        {
            scoreText.text = $"ניקוד: {score}";
        }

        if (timeText != null)
        {
            timeText.text = $"זמן: {timeTaken:F2} שניות";
        }
    }

    /// <summary>
    /// Retrieves the current game name based on the scene name.
    /// </summary>
    private string GetCurrentGameName()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        if (miniGamesAndStages != null)
        {
            foreach (var gameInfo in miniGamesAndStages.games)
            {
                foreach (var stageName in gameInfo.stageNames)
                {
                    if (stageName == currentSceneName)
                    {
                        return gameInfo.gameName;
                    }
                }
            }
        }

        Debug.LogWarning($"[ScoreManagerr] Game name not found for scene: {currentSceneName}");
        return null;
    }
}
