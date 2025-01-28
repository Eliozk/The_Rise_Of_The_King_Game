using UnityEngine;
using UnityEngine.UI;
using TMPro;  // For updating item count text

public class MazeProgress : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Image component of the progress bar.")]
    public Image progressBarImage;

    [Tooltip("Array of sprites representing different progress stages.")]
    public Sprite[] progressStages;

    [Tooltip("TextMeshProUGUI for displaying item count.")]
    public TextMeshProUGUI itemCounterText;

    [Header("Progress Tracking")]
    [Tooltip("Total number of collectible items.")]
    public int totalItems = 1;

    private int collectedItems = 0;
    private const int ONE = 1;
    private const int ZERO = 0;

    [Header("Audio Settings")]
    [Tooltip("Sound played when progress is updated.")]
    public AudioClip progressSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Set up AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Initialize the progress bar UI
        UpdateProgressBar();
        UpdateCounterUI();
    }

    /// <summary>
    /// Called when an item is collected.
    /// </summary>
    public void OnItemCollected()
    {
        collectedItems++;
        UpdateProgressBar();
        UpdateCounterUI();

        // Play progress sound if available
        if (progressSound != null)
        {
            audioSource.PlayOneShot(progressSound);
        }

        // Check if all items are collected
        if (collectedItems >= totalItems)
        {
            itemCounterText.text = "כל הפריטים נאספו! כעת תוכל לצאת מהמבוך ולנצח!!!";
        }
    }

    /// <summary>
    /// Updates the progress bar based on the number of collected items.
    /// </summary>
    private void UpdateProgressBar()
    {
        if (progressStages.Length > ZERO && progressBarImage != null)
        {
            int index = Mathf.Clamp(collectedItems, ZERO, progressStages.Length - ONE);
            progressBarImage.sprite = progressStages[index];
        }
    }

    /// <summary>
    /// Updates the item counter UI to display the remaining items.
    /// </summary>
    private void UpdateCounterUI()
    {
        int remaining = totalItems - collectedItems;
        itemCounterText.text = "פריטים שנותרו לאסוף לפתיחת יציאת המבוך: " + remaining;
    }
}
