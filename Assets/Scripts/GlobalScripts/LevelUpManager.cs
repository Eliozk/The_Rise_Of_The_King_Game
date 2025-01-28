using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

/// <summary>
/// Manages the Level-Up screen, including displaying the player's score,
/// transitioning to the next level, and quitting the game.
/// </summary>
public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private GameObject levelUpPic; // The Level-Up UI panel that appears between levels.
    [SerializeField] private TextMeshProUGUI scoreText; // UI element to display the player's score.
    [SerializeField] private TextMeshProUGUI timeText; // UI element to display the player's time.
    [SerializeField] private Button continueButton; // Button to proceed to the next level.
    [SerializeField] private Button quitButton; // Button to quit the game.
    [SerializeField] private Button nextButton; // Button to restart the game from the first level when we finish game.

    private int levelsAmount = 2; // Number of levels for each game (0,1,2)
    private SceneManagement sceneManagement;

    [Tooltip("Sound played when the level is completed.")]
    public AudioClip levelEndSound;

    [Header("Scriptable Objects")]
    [SerializeField] private ScoreData scoreData; // Reference to ScoreData Scriptable Object.
    [SerializeField] private TimeData timeData; // Reference to TimeData Scriptable Object.
    [SerializeField] private GameStats gameStats; // Reference to GameStats Scriptable Object.

    private bool magicalItemsDisplayed = false; // Ensures magical items are only displayed once per level.

    private void Start()
    {
        if (levelUpPic != null)
        {
            levelUpPic.SetActive(false); // Ensure the Level-Up screen is hidden initially.
        }

        continueButton.onClick.AddListener(ContinueToNextLevel); // Add listener to "Continue" button.

        sceneManagement = SceneManagement.Instance;
        if (sceneManagement == null)
        {
            Debug.LogError("[LevelUpManager] SceneManagement instance is null! Make sure it exists in the scene.");
        }
    }

    private void Awake()
    {
        if (levelUpPic == null)
        {
            levelUpPic = GameObject.Find("LevelUpPic");
            if (levelUpPic == null)
            {
                Debug.LogError("LevelUpPic could not be found in the scene.");
            }
        }
    }

    /// <summary>
    /// Shows the level-up screen and manages the magical items display.
    /// </summary>
    public void ShowLevelUpScreen(Action onComplete = null)
    {
        Debug.Log($"[LevelUpManager] Entering ShowLevelUpScreen - Called at {Time.time} seconds");

        if (levelUpPic == null)
        {
            Debug.LogError("[LevelUpManager] levelUpPic is not assigned!");
            onComplete?.Invoke();
            return;
        }

        if (!magicalItemsDisplayed)
        {
            Debug.Log("[LevelUpManager] Entering DisplayMagicalItems");

            DisplayMagicalItems(() =>
            {
            Debug.Log("[LevelUpManager] Entering DisplayMagicalItems - !magicalItemsDisplayed = true");

                magicalItemsDisplayed = true;
                DisplayLevelUpScreen(onComplete);
            });
        }
        else
        {
            Debug.Log("[LevelUpManager] Entering DisplayMagicalItems - !magicalItemsDisplayed = false");

            DisplayLevelUpScreen(onComplete);
        }
    }

    /// <summary>
    /// Displays the Level-Up UI and updates the score and time.
    /// </summary>
    private void DisplayLevelUpScreen(Action onComplete)
    {
        UpdateScoreAndTime();

        // Get current scene and game information
        string currentSceneName = SceneManager.GetActiveScene().name;
        string gameName = sceneManagement.GetCurrentGameName();

        // Fetch score and time
        int totalScore = scoreData.GetTotalScore(gameName);
        float totalTime = timeData.GetTotalTime(gameName);

        if (scoreText != null && timeText != null)
        {
            scoreText.text = $"{totalScore} :דוקינ";
            timeText.text = $"תוינש {totalTime:F2} :ןמז";
        }
        else
        {
            Debug.LogError("[LevelUpManager] UI elements (scoreText or timeText) are not assigned!");
        }

        // Check if all levels are complete
        if (AreAllLevelsComplete())
        {
            continueButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        levelUpPic.SetActive(true);

        // Play level completion sound
        if (levelEndSound != null)
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.PlayOneShot(levelEndSound);
            Debug.Log("[LevelUpManager] Level-Up sound played.");
        }
        // Delay the callback slightly to ensure the screen is visible
        StartCoroutine(WaitAndInvoke(0.5f, onComplete));
    }

    private IEnumerator WaitAndInvoke(float delay, Action callback)
    {
        yield return new WaitForSecondsRealtime(delay); // Wait without being affected by timeScale
        callback?.Invoke();
    }


    /// <summary>
    /// Displays magical items if they are unlocked.
    /// </summary>
    /// <param name="onComplete">Callback to proceed after magical items are displayed.</param>
    private void DisplayMagicalItems(Action onComplete)
    {
        var magicalItemDisplay = UnityEngine.Object.FindFirstObjectByType<MagicalItemDisplay>();

        if (magicalItemDisplay != null)
        {
            if (magicalItemsDisplayed)
            {
                // If magical items were already displayed, skip the duration wait
                Debug.Log("[LevelUpManager] Magical items already displayed, skipping to Level-Up screen.");
                onComplete?.Invoke();
            }
            else
            {
                magicalItemDisplay.CheckAndDisplayMagicalItems(() =>
                {
                    magicalItemsDisplayed = true;
                    Debug.Log("[LevelUpManager] Magical items display complete. Proceeding to Level-Up screen.");
                    onComplete?.Invoke();
                });
            }
        }
        else
        {
            Debug.LogError("[LevelUpManager] MagicalItemDisplay not found in the scene!");
            onComplete?.Invoke();
        }
    }

    public void ShowLevelUpScreenForScoreOnly()
    {
        if (levelUpPic == null)
        {
            Debug.LogError("[LevelUpManager] levelUpPic is not assigned!");
            return;
        }

        // Display magical items first, then proceed to show the levelUpPic
        var magicalItemDisplay = UnityEngine.Object.FindFirstObjectByType<MagicalItemDisplay>();
        if (magicalItemDisplay != null)
        {
            magicalItemDisplay.CheckAndDisplayMagicalItems(() =>
            {
                // Once magical items are displayed, proceed to show the level-up screen
                DisplayLevelUpPicForScoreOnly();
            });
        }
        else
        {
            Debug.LogError("[LevelUpManager] MagicalItemDisplay not found in the scene!");
            // If no magical items to display, directly show the level-up screen
            DisplayLevelUpPicForScoreOnly();
        }
    }

    private void DisplayLevelUpPicForScoreOnly()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string gameName = sceneManagement.GetCurrentGameName();

        int stageScore = scoreData.GetStageScore(gameName, currentSceneName);

        if (scoreText != null)
        {
            scoreText.text = $"{stageScore} :דוקינ";
            timeText.text = "";
        }
        else
        {
            Debug.LogError("[LevelUpManager] scoreText is not assigned!");
        }

        if (AreAllLevelsComplete())
        {
            continueButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        levelUpPic.SetActive(true);

        // Optional: Play level completion sound
        if (levelEndSound != null)
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.PlayOneShot(levelEndSound);
            Debug.Log("[LevelUpManager] Level-Up sound played.");
        }
    }


    /// <summary>
    /// Checks if all levels are complete.
    /// </summary>
    /// <returns>True if all levels are complete, false otherwise.</returns>
    public bool AreAllLevelsComplete()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return currentSceneIndex == levelsAmount;
    }

    /// <summary>
    /// Updates the score and time display.
    /// </summary>
    private void UpdateScoreAndTime()
    {
        string gameName = sceneManagement.GetCurrentGameName();
        int calculatedScore = SingletonManager.singltoneInstance.scoreData.GetTotalScore(gameName);
        float elapsedTime = SingletonManager.singltoneInstance.timeData.GetTotalTime(gameName);

        Debug.Log($"[LevelUpManager] Calculated Score: {calculatedScore}, Elapsed Time: {elapsedTime:F2}");
    }

    private void ContinueToNextLevel()
    {
        if (levelUpPic != null)
        {
            levelUpPic.SetActive(false);
        }
        sceneManagement.CompleteLevel();
    }
}
