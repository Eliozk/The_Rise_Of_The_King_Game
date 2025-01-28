using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles transitioning to the next scene after the portal effect is completed.
/// </summary>
public class NextScene : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("The name of the scene to load.")]   
    [SerializeField] private string sceneToLoad; // The name of the scene to load

    [Tooltip("Delay (in seconds) before transitioning to the next scene.")]
    [SerializeField] private float delayBeforeTransition; // Delay (in seconds) before loading the scene

    [Header("Scriptable Objects")]
    [Tooltip("Reference to the TimeData Scriptable Object.")]
    [SerializeField] private TimeData timeData;

    [Tooltip("Reference to the ScoreData Scriptable Object.")]
    [SerializeField] private ScoreData scoreData;

    [Tooltip("Reference to the GameStats Scriptable Object.")]
    [SerializeField]private GameStats gameStats;

//Ensuring gamestate not destroying accrossing different scenes
private void Awake()
{
    if (gameStats == null)
    {
        // Load GameStats from Resources
        gameStats = Resources.Load<GameStats>("ScriptableObjects/GameStats");
        if (gameStats != null)
        {
            Debug.Log("[NextScene] GameStats successfully loaded from Resources.");
        }
        else
        {
            Debug.Log("[NextScene] GameStats is missing and could not be found in Resources.");
        }
    }
}




    /// <summary>
    /// Transitions to the specified scene, resetting time and score if the current scene is part of a game stage.
    /// </summary>
    public void TransitionToScene()
    {
        StartCoroutine(LoadSceneWithReset());

    }

    /// <summary>
    /// Coroutine to handle the scene transition with optional reset of time and score.
    /// </summary>
    private IEnumerator LoadSceneWithReset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reset data if the current scene is part of a game stage
        if (IsPartOfGameStages(currentSceneName))
        {
            ResetCurrentLevelData(currentSceneName);
        }

        yield return new WaitForSeconds(delayBeforeTransition);
        Time.timeScale = 1; // Reset time scale to normal, ensuring the game is not paused before transition.

        // Load the next scene
        Debug.Log($"[SceneTransitionManager] Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
        // Rebind UI elements for ScoreManagerr
    }

    /// <summary>
    /// Resets the time and score data for the current level if it is part of a game.
    /// </summary>
    /// <param name="currentSceneName">The name of the current scene.</param>
    private void ResetCurrentLevelData(string currentSceneName)
    {
        if (gameStats == null)
        {
            Debug.LogError("[SceneTransitionManager] GameStats is not assigned.");
            return;
        }

        string gameName = GetGameName(currentSceneName);
        if (gameName == null)
        {
            Debug.LogWarning($"[SceneTransitionManager] No game found for current scene: {currentSceneName}");
            return;
        }

        // Reset time data
        if (timeData != null)
        {
            timeData.ResetStageTime(gameName, currentSceneName);
            Debug.Log($"[SceneTransitionManager] Time reset for stage: {currentSceneName}");
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] TimeData is not assigned.");
        }

        // Reset score data
        if (scoreData != null)
        {
            scoreData.ResetStageScores(gameName, currentSceneName);
            Debug.Log($"[SceneTransitionManager] Score reset for stage: {currentSceneName}");
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] ScoreData is not assigned.");
        }
    }

/// <summary>
    /// Retrieves the game name associated with the current stage.
    /// </summary>
    /// <param name="currentSceneName">The name of the current scene.</param>
    /// <returns>The name of the game.</returns>
    private string GetGameName(string currentSceneName)
    {
        if (gameStats == null)
        {
            Debug.LogError("[NextScene] GameStats is not assigned.");
            return null;
        }

        foreach (var game in gameStats.games)
        {
            foreach (var stage in game.stages)
            {
                if (stage.stageName == currentSceneName)
                {
                    return game.gameName;
                }
            }
        }

        Debug.LogWarning($"[NextScene] Game name not found for stage: {currentSceneName}");
        return null;
    }

    /// <summary>
    /// Checks if the current scene is part of any game stages.
    /// </summary>
    /// <param name="currentSceneName">The name of the current scene.</param>
    /// <returns>True if the scene is part of game stages; otherwise, false.</returns>
    private bool IsPartOfGameStages(string currentSceneName)
    {
        foreach (var game in gameStats.games)
        {
            foreach (var stage in game.stages)
            {
                if (stage.stageName == currentSceneName)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
