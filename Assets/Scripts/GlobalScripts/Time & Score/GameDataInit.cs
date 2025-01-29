using UnityEngine;

/// <summary>
/// Initializes game data by linking mini-games and stages to their score and time data.
/// </summary>
public class GameDataInitializer : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [Tooltip("Reference to the MiniGamesAndStages Scriptable Object.")]
    public MiniGamesAndStages miniGamesAndStages;

    [Tooltip("Reference to the ScoreData Scriptable Object.")]
    public ScoreData scoreData;

    [Tooltip("Reference to the TimeData Scriptable Object.")]
    public TimeData timeData;

    [Tooltip("Reference to the GameStats Scriptable Object.")]
    public GameStats gameStats;

    [Header("Game Settings")]
    [Tooltip("Maximum score a player can achieve per stage.")]
    [SerializeField] private int maxScore = 100; // Default max score 

    [Tooltip("Maximum time a player has to achieve a positive score.")]
    [SerializeField] private float maxTime = 60f; // Default max time 

    /// <summary>
    /// Initializes game data during the start of the scene.
    /// </summary>
    private void Start()
    {
        if (miniGamesAndStages == null || scoreData == null || timeData == null || gameStats == null)
        {
            Debug.LogError("[GameDataInitializer] Scriptable Objects are not assigned!");
            return;
        }
        InitializeGameData();
    }

    private void InitializeGameData()
    {
        // Initialize score data for the All the game
        scoreData.InitializeScoresToZero();
        // Initialize time data for All the game
        timeData.InitializeTimesToZero();
        // Initialize time data for All the game
        gameStats.InitializeStats();
        gameStats.ResetMagicalItemFlags();

    }

    /// <summary>
    /// Provides the maximum score allowed in the game.
    /// </summary>
    public int GetMaxScore()
    {
        return maxScore;
    }

    /// <summary>
    /// Provides the maximum time allowed for the game.
    /// </summary>
    public float GetMaxTime()
    {
        return maxTime;
    }
}
