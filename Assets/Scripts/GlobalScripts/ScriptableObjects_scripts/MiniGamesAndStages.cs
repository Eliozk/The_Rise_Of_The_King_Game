using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scriptable Object to store game and stage data.
/// </summary>
[CreateAssetMenu(fileName = "MiniGamesAndStages", menuName = "Game Data/Mini Games and Stages")]
public class MiniGamesAndStages : ScriptableObject
{
    /// <summary>
    /// Represents information about a game and its stages.
    /// </summary>
    [System.Serializable]
    public class GameInfo
    {
        public string gameName; // The name of the game.
        public List<string> stageNames; // List of stage names for the game.
    }

    [Tooltip("List of games and their stages.")]
    public List<GameInfo> games = new List<GameInfo>();
}
