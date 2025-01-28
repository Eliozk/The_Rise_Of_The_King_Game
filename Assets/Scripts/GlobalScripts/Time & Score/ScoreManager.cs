using UnityEngine;
using TMPro;

/// <summary>
/// Manages and displays scores for mini-games and their stages.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [Tooltip("Reference to the ScoreData Scriptable Object.")]
    public ScoreData scoreData;

    [Header("UI Elements")]
    [Tooltip("TextMeshPro element for displaying the score.")]
    public TMP_Text scoreText;

   /// <summary>
    /// Updates the score for a specific stage in a mini-game and displays it.
    /// </summary>
    /// <param name="gameName">The name of the mini-game.</param>
    /// <param name="stageName">The name of the stage.</param>
    /// <param name="score">The score to update.</param>
    public void UpdateScore(string gameName, string stageName, int score)
    {
        if (scoreData != null)
        {
            scoreData.UpdateScore(gameName, stageName, score);
            DisplayScore(gameName);
        }
        else
        {
            Debug.LogError("[ScoreManager] ScoreData is not assigned!");
        }
    }

     /// <summary>
    /// Displays the total score for a specific mini-game.
    /// </summary>
    /// <param name="gameName">The name of the mini-game.</param>
    public void DisplayScore(string gameName)
    {
        if (scoreData != null)
        {
            int totalScore = scoreData.GetTotalScore(gameName);
            scoreText.text = $"Game: {gameName}\nTotal Score: {totalScore}";
        }
        else
        {
            Debug.LogError("[ScoreManager] ScoreData is not assigned!");
        }
    }
}
