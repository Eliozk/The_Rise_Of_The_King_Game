using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Collections;

/// <summary>
/// Manages and displays times for mini-games and their stages.
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [Tooltip("Reference to the TimeData Scriptable Object.")]
    public TimeData timeData;

    [Header("UI Elements")]
    [Tooltip("TextMeshPro element for displaying the time.")]

    [Header("Watch Stage Real Time")]
    [SerializeField, ReadOnly] private float elapsedTime; // Debug display for elapsed time
    public TMP_Text timeText;
    //start measure time of scene
    private float startTime;


    private void Start()
    {
        startTime = Time.time; // Start measuring time
        Debug.Log($"[TimeManager] Started timing for scene: {SceneManager.GetActiveScene().name}");
    }

    private void Update()
    {
        // Update elapsed time for debugging
        elapsedTime = Time.time - startTime;
    }

    /// <summary>
    /// Updates the time for a specific stage in a mini-game and displays it.
    /// </summary>
    /// <param name="gameName">The name of the mini-game.</param>
    /// <param name="stageName">The name of the stage (scene).</param>
    /// <param name="timeTaken">The time taken to complete the stage.</param>
    public void UpdateTime(string gameName, string stageName, float timeTaken)
    {
        // Call the updated UpdateTime function in TimeData
        timeData.UpdateTime(gameName, stageName, timeTaken);

        // Display the updated total time for the mini-game
        DisplayTime(gameName);
    }

    /// <summary>
    /// Displays the total time for a specific mini-game.
    /// </summary>
    /// <param name="gameName">The name of the mini-game.</param>
    public void DisplayTime(string gameName)
    {
        float totalTime = timeData.GetTotalTime(gameName);
        timeText.text = $"Game: {gameName}\nTotal Time: {totalTime:F2} seconds";
    }
}
