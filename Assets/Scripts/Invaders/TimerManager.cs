using UnityEngine;
using TMPro; // TextMeshPro for displaying the timer
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance; // Singleton instance of the TimerManager
    [SerializeField] private TextMeshProUGUI timerText; // UI element for displaying the timer
    [SerializeField] private float levelTime = 30f; // Total level time in seconds
    private float currentTime; // Current countdown time
    private int zero = 0; // Constant for zero to avoid magic numbers
    private bool levelCompleted = false; // Flag to ensure level completion logic runs only once


    private LevelUpManager levelUpManager; // Reference to the LevelUpManager
    private ScoreManagerInvaders scoreManagerInvaders; // Reference to the ScoreManagerInvaders


    private void Awake()
    {
        // Ensure only one instance of TimerManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate TimerManager objects
        }
    }

    private void Start()
    {
        // Find the LevelUpManager in the scene
        levelUpManager = Object.FindFirstObjectByType<LevelUpManager>();
        scoreManagerInvaders = Object.FindFirstObjectByType<ScoreManagerInvaders>();

        if (levelUpManager == null)
        {
            Debug.LogError("[TimerManager] LevelUpManager could not be found in the scene!");
        }
        if (scoreManagerInvaders == null)
        {
            Debug.LogError("[TimerManager] ScoreManagerInvaders could not be found in the scene!");
        }

        ResetTimer(); // Initialize the timer at the start of the level
    }

    private void Update()
    {
        HandleTimer(); // Update the timer every frame
    }

    private void HandleTimer()
    {
        // If there is remaining time, decrement it and update the display
        if (currentTime > zero)
        {
            currentTime -= Time.deltaTime; // Reduce the time by the elapsed frame time
            timerText.text = Mathf.Ceil(currentTime).ToString() + " :רתונש ןמז "; // Update the timer UI
        }
        else if(!levelCompleted)
        {
            levelCompleted = true; // Set the flag to prevent repeated level completion

            // If time reaches zero, complete the level
            currentTime = zero;
            LevelComplete();
        }
    }

    public void ResetTimer()
    {
        // Reset the timer to the total level time
        currentTime = levelTime;
        levelCompleted = false; // Reset the level completion flag

    }
    private void LevelComplete()
    {
        // Stop spawning characters when the level is complete
        CharacterSpawner.Instance.StopSpawning();

         // Destroy all characters with the "King" tag
    GameObject[] kingCharacters = GameObject.FindGameObjectsWithTag("King");
    if (kingCharacters != null && kingCharacters.Length > zero)
    {
        foreach (GameObject character in kingCharacters)
        {
            Destroy(character);
        }
    }
    else
    {
        Debug.Log("[TimerManager] No objects found with the tag 'King'.");
    }

    // Destroy all characters with the "Robber" tag
    GameObject[] robberCharacters = GameObject.FindGameObjectsWithTag("Robber");
    if (robberCharacters != null && robberCharacters.Length > zero)
    {
        foreach (GameObject character in robberCharacters)
        {
            Destroy(character);
        }
    }
    else
    {
        Debug.Log("[TimerManager] No objects found with the tag 'Robber'.");
    }
        // Trigger the LevelUpManager to show the LevelUpPic
        if (levelUpManager != null)
        {
            scoreManagerInvaders.FinalizeScore(); // call finalize function to update score and shoe results on screen
        }
        else
        {
            Debug.LogError("[TimerManager] LevelUpManager is not assigned or found in the scene.");
        }
    }


}
