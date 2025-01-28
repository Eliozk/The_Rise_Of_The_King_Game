using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to manage scores for all mini-games and their stages.
/// </summary>
[CreateAssetMenu(fileName = "NewScoreData", menuName = "Game/Score Data")]
public class ScoreData : ScriptableObject
{
    [System.Serializable]
    public class StageScore
    {
        public string stageName; // Name of the stage.
        public int score;        // Score for the stage.
    }

    [System.Serializable]
    public class GameScore
    {
        public string gameName; // Name of the mini-game.
        public List<StageScore> stageScores = new List<StageScore>(); // List of stage scores.
    }

    public List<GameScore> gameScores = new List<GameScore>(); // List to store game scores.

    /// <summary>
    /// Initializes all stage scores for existing games to 0.
    /// Ensures that all games and their stages start with a score of 0.
    /// </summary>
    public void InitializeScoresToZero()
    {
        foreach (var game in gameScores)
        {
            foreach (var stage in game.stageScores)
            {
                stage.score = 0; // Reset the score for all stages
            }
            Debug.Log($"[ScoreData] Initialized scores for game '{game.gameName}' to 0.");
        }
    }


    /// <summary>
    /// Updates the score for a specific stage in a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <param name="stageName">Name of the stage.</param>
    /// <param name="score">Score to update.</param>
    public void UpdateScore(string gameName, string stageName, int score)
    {
        var game = gameScores.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stageScores.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                stage.score = score;
            }
            else
            {
                Debug.LogWarning($"Stage '{stageName}' not found in game '{gameName}'.");
            }


        }
        else
        {
            Debug.LogWarning($"Game '{gameName}' not found in ScoreData.");
        }
    }

    /// <summary>
    /// Resets the score for a specific stage in a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <param name="stageName">Name of the stage to reset.</param>
    public void ResetStageScores(string gameName, string stageName)
    {
        var game = gameScores.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stageScores.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                stage.score = 0;
                Debug.Log($"Score reset for stage '{stageName}' in game '{gameName}'.");
            }
            else
            {
                Debug.LogWarning($"Stage '{stageName}' not found in game '{gameName}'.");
            }
        }
        else
        {
            Debug.LogWarning($"Game '{gameName}' not found in ScoreData.");
        }
    }


    /// <summary>
    /// Calculates the total score for a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <returns>Total score for the mini-game.</returns>
    public int GetTotalScore(string gameName)
    {
        var game = gameScores.Find(g => g.gameName == gameName);
        if (game != null)
        {
            int totalScore = 0;
            foreach (var stage in game.stageScores)
            {
                totalScore += stage.score;
            }
            return totalScore;
        }
        Debug.LogWarning($"Game '{gameName}' not found in ScoreData.");
        return 0;
    }

    /// <summary>
    /// Calculates the score for a specific stage of a mini-game.
    /// </summary>
    /// <param name="gameName">Name of the mini-game.</param>
    /// <param name="stageName">Name of the stage.</param>
    /// <returns>Score for the specified stage.</returns>
    public int GetStageScore(string gameName, string stageName)
    {
        var game = gameScores.Find(g => g.gameName == gameName);
        if (game != null)
        {
            var stage = game.stageScores.Find(s => s.stageName == stageName);
            if (stage != null)
            {
                return stage.score;
            }
            else
            {
                Debug.LogWarning($"Stage '{stageName}' not found in game '{gameName}'.");
            }
        }
        else
        {
            Debug.LogWarning($"Game '{gameName}' not found in ScoreData.");
        }
        return 0; // Return 0 if the game or stage is not found
    }

}
