using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private static bool hasTriggeredGlobally = false; // Static flag to ensure the trigger logic runs once globally

    private void Start()
    {
        // Reset the global trigger at the start of the scene
        ResetGlobalTrigger();
    }
    // This method is triggered when another collider enters the trigger area of this object.
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggeredGlobally)
        {
            Debug.Log("[ExitTrigger] Trigger already activated, ignoring further calls.");
            return; // Ignore if the trigger has already been activated
        }
        Debug.Log($"[ExitTrigger] OnTriggerEnter called with {other.name} at {Time.time} seconds");
        // Check if the collider is the player by comparing tags
        if (other.CompareTag("Player"))
        {
            // Find the item manager in the scene to check item collection status
            var itemManager = FindFirstObjectByType<itemsScript>();
            // Ensure the item manager is found and check if all items have been collected
            if (itemManager != null && itemManager.AllItemsCollected())
            {
                // If all items are collected, print that the maze is completed
                // Set the global flag to true
                hasTriggeredGlobally = true;
                Debug.Log("Maze Completed!");
                // End the level
                var mazeScore = Object.FindFirstObjectByType<MazeScore>();
                if (mazeScore != null)
                {
                    mazeScore.EndLevel();
                }
                else
                {
                    Debug.LogError("[ExitTrigger] ScoreManagerr not found in the scene.");
                }
                // Notify scene transition by showing the Level-Up screen
                var levelUpManager = Object.FindFirstObjectByType<LevelUpManager>();
                if (levelUpManager != null)
                {
                    // Pass a callback to PauseGame after the LevelUp screen is displayed
                    levelUpManager.ShowLevelUpScreen(() =>
                    {
                        PauseGame();
                    });

                }
                else
                {
                    Debug.LogError("[ExitTrigger] LevelUpManager not found in the scene.");
                }
            }
            else
            {
                // Calculate how many more items the player needs to collect to exit
                int remainingItems = itemManager.totalItems - itemManager.collectedItems;

                // print remaining items to inform the player that they need to collect more
                Debug.Log("Collect " + remainingItems + " more item(s) to unlock the exit!");
            }
        }
    }

    private void PauseGame()
    {
        Debug.Log("[ExitTrigger] Pausing game.");
        Time.timeScale = 0; // Pauses everything related to time (animations, movements based on deltaTime, etc.)
        Debug.Log("Game Paused!");

    }

    //  Reset the global trigger (for debugging or reloading levels)
    public static void ResetGlobalTrigger()
    {
        hasTriggeredGlobally = false;
        Debug.Log("[ExitTrigger] Global trigger reset.");
    }
}
