using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scriptable Object to manage game stats (score and time) for each stage in the game.
/// </summary>
[CreateAssetMenu(fileName = "GameStats", menuName = "Game/Data/Game Stats")]
public class GameStats : ScriptableObject
{
    [System.Serializable]
    public class StageStats
    {
        public string stageName; // The name of the stage (scene name).
        public int score; // Current score for the stage.
        public float time; // Current time spent in the stage.
        public int bestScore; // Best score ever achieved in this stage.
        public float bestTime; // Best time ever achieved in this stage.
    }

    [System.Serializable]
    public class GameStatsData
    {
        public string gameName; // The name of the game.
        public List<StageStats> stages = new List<StageStats>(); // Stats for each stage in the game.

    }

    [Tooltip("List of games and their stats.")]
    public List<GameStatsData> games = new List<GameStatsData>();

    // Boolean indicators for unlocking magical items
    public bool diamond;
    public bool witchStick;
    public bool crown;

    // **New Boolean Flags for Global Display Tracking**
    public bool isDiamondDisplayed = false;
    public bool isWitchStickDisplayed = false;
    public bool isCrownDisplayed = false;

    [Header("Score Thresholds")]
    [Tooltip("Threshold score to unlock the diamond item.")]
    public int diamondThreshold = 50;

    [Tooltip("Threshold score to unlock the witch stick item.")]
    public int witchStickThreshold = 50;

    [Tooltip("Threshold score to unlock the crown item.")]
    public int crownThreshold = 50;

    /// <summary>
    /// Initializes stats for all games and stages.
    /// </summary>
    public void InitializeStats()
    {
        foreach (var gameData in games)
        {
            foreach (var stage in gameData.stages)
            {
                stage.score = 0;
                stage.time = 0f;
                stage.bestScore = 0;
                stage.bestTime = float.MaxValue;
            }
        }
    }

    /// <summary>
    /// Resets all magical item flags for all games.
    /// </summary>
    public void ResetMagicalItemFlags()
    {
        // Initialize magical item indicators
        diamond = false;
        witchStick = false;
        crown = false;
        isDiamondDisplayed = false;
        isWitchStickDisplayed = false;
        isCrownDisplayed = false;

    }

    /// <summary>
    /// Updates the stats for a specific stage in a game.
    /// </summary>
    /// <param name="gameName">Name of the game.</param>
    /// <param name="stageName">Name of the stage (scene).</param>
    /// <param name="newScore">New score to update.</param>
    /// <param name="newTime">New time to update.</param>
    public void UpdateStageStats(string gameName, string stageName, int newScore, float newTime)
    {
        var game = games.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stages.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                stage.score = newScore;
                stage.time = newTime;

                // Update best score and time if the new values are better.
                if (newScore > stage.bestScore)
                {
                    stage.bestScore = newScore;
                }
                if (newTime < stage.bestTime)
                {
                    stage.bestTime = newTime;
                }

                // Check and update magical item indicators
                UpdateMagicalItems(game);
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
    /// Updates the magical item indicators based on the total best score of a game.
    /// </summary>
    /// <param name="game">The game data to check.</param>
    private void UpdateMagicalItems(GameStatsData game)
    {
        int totalBestScore = 0;

        // Calculate the total best score from all stages
        foreach (var stage in game.stages)
        {
            totalBestScore += stage.bestScore; // Use bestScore instead of score
        }

        // Update indicators based on thresholds for specific games
        if (game.gameName == "MazeGame")
        {
            witchStick = totalBestScore >= witchStickThreshold;
        }
        if (game.gameName == "ArrangeGame")
        {
            diamond = totalBestScore >= diamondThreshold;
        }
        if (game.gameName == "DefendGame")
        {
            crown = totalBestScore >= crownThreshold;
        }

        Debug.Log($"[GameStats] Magical Items Updated for {game.gameName}: TotalBestScore={totalBestScore}, Diamond={diamond}, WitchStick={witchStick}, Crown={crown}");
    }

}
