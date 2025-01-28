using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays magical items in the End Scene based on the player's achievements.
/// </summary>
public class EndSceneDisplay : MonoBehaviour
{
    [Header("Magical Item UI")]
    [Tooltip("Images for magical items.")]
    public Image[] magicalItemImages;

    [Tooltip("Sprites for magical items.")]
    public Sprite[] magicalItemSprites;

    [Header("Scriptable Object")]
    [Tooltip("Reference to the GameStats ScriptableObject.")]
    public GameStats gameStats;

    private void Start()
    {
        UpdateEndScene();
    }

    /// <summary>
    /// Updates the End Scene UI based on the player's achievements.
    /// </summary>
    private void UpdateEndScene()
    {
        if (gameStats == null)
        {
            Debug.LogError("[EndSceneDisplay] GameStats ScriptableObject is not assigned!");
            return;
        }

        for (int i = 0; i < magicalItemImages.Length; i++)
        {
            bool shouldDisplay = CheckIfMagicalItemUnlocked(i);

            if (shouldDisplay && i < magicalItemSprites.Length)
            {
                magicalItemImages[i].sprite = magicalItemSprites[i];
                magicalItemImages[i].gameObject.SetActive(true);
            }
            else
            {
                magicalItemImages[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Checks if a magical item is unlocked based on the total score for a specific game.
    /// </summary>
    /// <param name="itemIndex">Index of the magical item.</param>
    /// <returns>True if the item is unlocked, otherwise false.</returns>
    private bool CheckIfMagicalItemUnlocked(int itemIndex)
    {
        string currentGameName = SceneManager.GetActiveScene().name;
        int totalScore = 0;

        var game = gameStats.games.Find(g => g.gameName == currentGameName);
        if (game != null && itemIndex < game.stages.Count)
        {
            foreach (var stage in game.stages)
            {
                totalScore += stage.score;
            }
        }

        return totalScore >= 8000; // Replace with your desired score threshold
    }
}
