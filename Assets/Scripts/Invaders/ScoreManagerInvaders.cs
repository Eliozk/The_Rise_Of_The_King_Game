using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


/// <summary>
/// Manages the player's score during the game. 
/// Handles adding and subtracting points based on actions, 
/// updates the ScriptableObject for score data, 
/// and integrates with LevelUpManager to display the score at the end of the level.
/// </summary>
public class ScoreManagerInvaders : MonoBehaviour
{
    [Header("Score Settings")]

    /// <summary>
    /// Tracks the player's current score.
    /// </summary>
    private int currentScore = 0;
    private float noTime = 0f; // in invaders there is no value for timing reasults therefore update always time to 0.

    /// <summary>
    /// Points awarded for a correct action.
    /// </summary>
    [SerializeField] private int pointsPerCorrectAction = 10;

    /// <summary>
    /// Points deducted for an incorrect action.
    /// </summary>
    [SerializeField] private int pointsPerIncorrectAction = -5;
    private const int MinScore = 0; // Define a constant for the minimum score

    [Header("Scriptable Objects")]
    /// <summary>
    /// Reference to the ScriptableObject that stores score data.
    /// </summary>
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private GameStats gameStats;

    private LevelUpManager levelUpManager; // Reference to LevelUpManager
    private string gameName; // Name of the current game
    private string currentSceneName; // Name of the current scene


    private void Start()
    {
        // Retrieve game and scene names from SceneManagement
        var sceneManagement = SceneManagement.Instance;
        if (sceneManagement != null)
        {
            gameName = sceneManagement.GetCurrentGameName();
            currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        else
        {
            Debug.LogError("[ScoreManager] SceneManagement is not found!");
        }

        // Dynamically find the LevelUpManager in the scene
        if (levelUpManager == null)
        {
            levelUpManager = Object.FindFirstObjectByType<LevelUpManager>(); // Updated to use FindFirstObjectByType
            if (levelUpManager == null)
            {
                Debug.LogError("[ScoreManager] LevelUpManager could not be found in the scene!");
            }
        }

        // Ensure the ScoreData ScriptableObject is assigned
        if (scoreData == null)
        {
            Debug.LogError("[ScoreManager] ScoreData ScriptableObject is not assigned!");
        }
        if (gameStats == null)
        {
            Debug.LogError("[ScoreManager] GameStats ScriptableObject is not assigned!");
        }

    }

    /// <summary>
    /// Adds points for a correct action.
    /// </summary>
    public void AddScore()
    {
        currentScore += pointsPerCorrectAction;
        Debug.Log($"[ScoreManager] Added {pointsPerCorrectAction} points. Current score: {currentScore}");
        // Update the ScriptableObject
        if (scoreData != null)
        {
            scoreData.UpdateScore(gameName, currentSceneName, currentScore);
        }
    }

    /// <summary>
    /// Subtracts points for an incorrect action.
    /// </summary>
    public void SubtractScore()
    {
        currentScore += pointsPerIncorrectAction;

        // Ensure the score does not drop below the minimum
        if (currentScore < MinScore)
        {
            currentScore = MinScore;
        }

        Debug.Log($"[ScoreManager] Subtracted {Mathf.Abs(pointsPerIncorrectAction)} points. Current score: {currentScore}");

        // Update the ScriptableObject
        if (scoreData != null)
        {
            scoreData.UpdateScore(gameName, currentSceneName, currentScore);
        }
    }

    /// <summary>
    /// Finalizes the score, updates the LevelUpManager, and displays the score at the end of the level.
    /// </summary>
    public void FinalizeScore()
    {
        // Update the ScriptableObject with the final score
        if (scoreData != null)
        {
            scoreData.UpdateScore(gameName, currentSceneName, currentScore);
            gameStats.UpdateStageStats(gameName, currentSceneName, currentScore, noTime);

        }

        // Display score in LevelUpManager
        if (levelUpManager != null)
        {
            levelUpManager.ShowLevelUpScreenForScoreOnly(); // Display only score
        }
        else
        {
            Debug.LogError("[ScoreManager] LevelUpManager is not found or not assigned!");
        }
    }
}
