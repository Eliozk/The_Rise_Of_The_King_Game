using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject to manage time tracking for all mini-games and their stages.
/// </summary>
[CreateAssetMenu(fileName = "TimeData", menuName = "Game/Time Data")]
public class TimeData : ScriptableObject
{
    /// <summary>
    /// Represents the time tracking for a specific stage in a game.
    /// </summary>
    [System.Serializable]
    public class StageTime
    {
        public string stageName; // Name of the stage (scene).
        public float time; // Time spent in the stage.
    }

    /// <summary>
    /// Represents time tracking for a specific mini-game.
    /// </summary>
    [System.Serializable]
    public class GameTime
    {
        public string gameName; // Name of the mini-game.
        public List<StageTime> stageTimes = new List<StageTime>(); // List of times for each stage.
        public float totalTime; // Total time for the mini-game.

        /// <summary>
        /// Recalculates the total time for the mini-game based on its stages.
        /// </summary>
        public void CalculateTotalTime()
        {
            totalTime = 0f;
            foreach (var stage in stageTimes)
            {
                totalTime += stage.time;
            }
        }
    }

    [Tooltip("List of games and their times.")]
    public List<GameTime> gameTimes = new List<GameTime>();

    /// <summary>
    /// Initializes time tracking for all stages based on the data in MiniGamesAndStages.
    /// This method ensures that only existing games and stages in TimeData are updated.
    /// </summary>
    /// <param name="miniGamesAndStages">Reference to the MiniGamesAndStages ScriptableObject.</param>
    /// <summary>
    /// Initializes all stage times for existing games to 0.
    /// Ensures that all games and their stages start with a time of 0.
    /// </summary>
    public void InitializeTimesToZero()
    {
        foreach (var game in gameTimes)
        {
            foreach (var stage in game.stageTimes)
            {
                stage.time = 0f; // Reset the time for all stages
            }
            game.CalculateTotalTime(); // Recalculate total time for the game
            Debug.Log($"[TimeData] Initialized times for game '{game.gameName}' to 0.");
        }
    }
    /// <summary>
    /// Updates the time for a specific stage in a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <param name="stageName">Name of the stage (scene).</param>
    /// <param name="timeTaken">Time to update for the stage.</param>
    public void UpdateTime(string gameName, string stageName, float timeTaken)
    {
        var game = gameTimes.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stageTimes.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                stage.time = timeTaken;
                game.CalculateTotalTime();
            }
            else
            {
                Debug.LogWarning($"Stage {stageName} not found in game {gameName}.");
            }
        }
        else
        {
            Debug.LogWarning($"Game {gameName} not found.");
        }
    }

    /// <summary>
    /// Resets the time for a specific stage in a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <param name="stageName">Name of the stage to reset.</param>
    public void ResetStageTime(string gameName, string stageName)
    {
        var game = gameTimes.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stageTimes.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                stage.time = 0f;
                game.CalculateTotalTime();
            }
            else
            {
                Debug.LogWarning($"Stage {stageName} not found in game {gameName}.");
            }
        }
        else
        {
            Debug.LogWarning($"Game {gameName} not found.");
        }
    }

    /// <summary>
    /// Gets the total time for a specific mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <returns>Total time for the mini-game.</returns>
    public float GetTotalTime(string gameName)
    {
        var game = gameTimes.Find(g => g.gameName == gameName);
        if (game != null)
        {
            return game.totalTime;
        }
        return 0f; // If the game is not found.
    }

    /// <summary>
    /// Resets all times for a specific mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    public void ResetGameTime(string gameName)
    {
        var game = gameTimes.Find(g => g.gameName == gameName);
        if (game != null)
        {
            foreach (var stage in game.stageTimes)
            {
                stage.time = 0f;
            }
            game.CalculateTotalTime();
        }
        else
        {
            Debug.LogWarning($"Game {gameName} not found.");
        }
    }
}
