using UnityEngine;

public class visibilityInEndRoom : MonoBehaviour
{
    [Header("Game Stats Reference")]
    [Tooltip("Scriptable Object reference to GameStats.")]
    [SerializeField] private GameStats gameStats;

    [Header("Image References")]
    [Tooltip("Image for the Diamond.")]
    [SerializeField] private GameObject diamondImage;

    [Tooltip("Image for the Witch Stick.")]
    [SerializeField] private GameObject witchStickImage;

    [Tooltip("Image for the Crown.")]
    [SerializeField] private GameObject crownImage;

    [Header("Button Reference")]
    [Tooltip("Button to display only if all items are visible.")]
    [SerializeField] private GameObject specialButton;

    private void Start()
    {
        if (gameStats == null)
        {
            Debug.LogError("[SimpleVisibilityManager] GameStats is not assigned!");
            return;
        }

        UpdateVisibility();
    }

    /// <summary>
    /// Updates the visibility of images based on the boolean values in GameStats.
    /// Also checks if all items are visible to activate the special button.
    /// </summary>
    private void UpdateVisibility()
    {
        bool allVisible = true;

        if (diamondImage != null)
        {
            diamondImage.SetActive(gameStats.diamond);
            if (!gameStats.diamond) allVisible = false;
        }
        else
        {
            Debug.LogWarning("[SimpleVisibilityManager] Diamond Image is not assigned!");
        }

        if (witchStickImage != null)
        {
            witchStickImage.SetActive(gameStats.witchStick);
            if (!gameStats.witchStick) allVisible = false;
        }
        else
        {
            Debug.LogWarning("[SimpleVisibilityManager] Witch Stick Image is not assigned!");
        }

        if (crownImage != null)
        {
            crownImage.SetActive(gameStats.crown);
            if (!gameStats.crown) allVisible = false;
        }
        else
        {
            Debug.LogWarning("[SimpleVisibilityManager] Crown Image is not assigned!");
        }

        // Show or hide the button based on allVisible
        if (specialButton != null)
        {
            specialButton.SetActive(allVisible);
        }
        else
        {
            Debug.LogWarning("[SimpleVisibilityManager] Special Button is not assigned!");
        }
    }
}
