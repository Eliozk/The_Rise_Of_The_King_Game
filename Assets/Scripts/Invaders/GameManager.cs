using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public int lives = 3; // Number of lives for the player
    [SerializeField] private int liveCounter = 3;
    [SerializeField] private int SecondsToWaitBeforeGameOver = 1;

    private int zero = 0;
    // Reference to LifeManager, TimerManager, and CharacterSpawner
    private LifeManager lifeManager;
    private TimerManager timerManager;
    private CharacterSpawner characterSpawner;

    // Audio Management
    [SerializeField] private AudioSource audioSource; // AudioSource component for playing sounds
    [SerializeField] private AudioClip positiveSound; // AudioClip for positive feedback
    [SerializeField] private AudioClip negativeSound; // AudioClip for negative feedback

    private void Awake()
    {
        // Dynamically find references to other components in the scene
        lifeManager = Object.FindFirstObjectByType<LifeManager>();
        timerManager = Object.FindFirstObjectByType<TimerManager>();
        characterSpawner = Object.FindFirstObjectByType<CharacterSpawner>();

        // Log any missing components
        if (lifeManager == null)
            Debug.LogError("[GameManager] LifeManager could not be found in the scene!");

        if (timerManager == null)
            Debug.LogError("[GameManager] TimerManager could not be found in the scene!");

        if (characterSpawner == null)
            Debug.LogError("[GameManager] CharacterSpawner could not be found in the scene!");

        // Ensure AudioSource is available
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void LoseLife()
    {
        if (lives > zero)
        {
            lives--;
            lifeManager.UpdateHearts(lives); // Update hearts using LifeManager
            PlayNegativeSound(); // Play negative feedback sound
        }

        if (lives <= zero)
        {
            StartCoroutine(WaitAndLoadGameOverScene()); // Start the coroutine to wait before loading the scene
            //GameOver();
        }
    }
    // Coroutine to wait for the negative sound to finish before transitioning to the Game Over scene
    private IEnumerator WaitAndLoadGameOverScene()
    {
        PlayNegativeSound(); // Play the negative sound
        yield return new WaitForSeconds(SecondsToWaitBeforeGameOver); // Wait for 3 seconds

        GameOver(); // Execute the GameOver function that handles game-ending logic

        // Now load the game over scene, assuming "GameOverScene" is the name of your scene
        SceneManager.LoadScene("GameOverScene");
    }

    public void PlayNegativeSound()
    {
        PlaySound(negativeSound);
    }
    // Method to play sounds from GameManager
    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        characterSpawner.StopSpawning(); // Stop spawning characters
        SceneManager.LoadScene("GameOverScene");
    }

    public void ResetGame()
    {
        // Reset lives
        lives = liveCounter;

        // Reset UI and components
        if (lifeManager != null)
        {
            lifeManager.ResetHearts();
        }

        if (timerManager != null)
        {
            timerManager.ResetTimer();
        }

        if (characterSpawner != null)
        {
            characterSpawner.StartSpawning();
        }
    }

}
