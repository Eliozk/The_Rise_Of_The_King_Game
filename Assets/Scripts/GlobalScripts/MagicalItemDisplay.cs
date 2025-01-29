using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MagicalItemDisplay : MonoBehaviour
{
    [Header("Magical Item Settings")]
    [Tooltip("Parent image that holds the magical items.")]
    public GameObject magicalItemContainer;

    [Tooltip("Images for magical items (Diamond, Witch Stick, Crown).")]
    public GameObject diamondImage;
    public GameObject witchStickImage;
    public GameObject crownImage;

    [Tooltip("Duration in seconds to display the magical items.")]
    public float displayDuration = 4f;

    [Tooltip("Particle effect for magical items.")]
    public ParticleSystem magicalItemEffect;

    [Tooltip("Audio clip for magical items display.")]
    public AudioClip magicalItemAudio;

    [Tooltip("Scriptable Object reference to GameStats.")]
    public GameStats gameStats;
    private AudioSource audioSource;
    private SceneManagement sceneManagement;

    private void Start()
    {
        HideMagicalDisplay();
        // CheckAndDisplayMagicalItems();
    }

    private void Awake()
    {
        // Ensure sceneManagement is assigned
        sceneManagement = SceneManagement.Instance;

        if (sceneManagement == null)
        {
            Debug.LogError("[MagicalItemDisplay] SceneManagement instance is null! Make sure SceneManagement exists in the scene.");
        }
        // Set up the audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Checks the GameStats ScriptableObject and displays magical items accordingly.
    /// </summary>
    public void CheckAndDisplayMagicalItems(Action onComplete)
    {
        if (gameStats == null)
        {
            Debug.LogError("[MagicalItemDisplay] GameStats ScriptableObject is not assigned!");
            onComplete?.Invoke();
            return;
        }
        if (onComplete == null)
        {
            Debug.LogError("[MagicalItemDisplay] onComplete callback is null!");
        }


        // Find the current game data
        string gameName = sceneManagement.GetCurrentGameName(); ;
        var game = gameStats.games.Find(g => g.gameName == gameName);

        if (game != null)
        {
            DisplayMagicalItems(game, onComplete);
        }
        else
        {
            Debug.LogWarning($"[MagicalItemDisplay] Game {gameName} not found in GameStats.");
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Displays magical items based on the boolean flags in the GameStats.
    /// Only displays items if they are being unlocked for the first time.
    /// </summary>
    /// <param name="game">The game data from GameStats.</param>
    private void DisplayMagicalItems(GameStats.GameStatsData game, Action onComplete)
    {
        bool hasDisplayed = false;

        if (gameStats.diamond && !gameStats.isDiamondDisplayed && diamondImage != null)
        {
            Debug.Log("[MagicalItemDisplay] Displaying Diamond");

            diamondImage.SetActive(true);
            gameStats.isDiamondDisplayed = true; // Mark as displayed globally
            hasDisplayed = true;
        }
        if (gameStats.witchStick && !gameStats.isWitchStickDisplayed && witchStickImage != null)
        {
            Debug.Log("[MagicalItemDisplay] Displaying Diamond");

            witchStickImage.SetActive(true);
            gameStats.isWitchStickDisplayed = true; // Mark as displayed globally
            hasDisplayed = true;
        }
        if (gameStats.crown && !gameStats.isCrownDisplayed && crownImage != null)
        {
            Debug.Log("[MagicalItemDisplay] Displaying Crown");

            crownImage.SetActive(true);
            gameStats.isCrownDisplayed = true; // Mark as displayed globally
            hasDisplayed = true;
        }

        if (hasDisplayed)
        {
            Debug.Log("[MagicalItemDisplay] Has Displayed block ENTERED");

            if (gameStats.witchStick) { witchStickImage.SetActive(true); }
            if (gameStats.diamond) { diamondImage.SetActive(true); }
            if (gameStats.crown) { crownImage.SetActive(true); }

            Debug.Log($"[MagicalItemDisplay] magicalItemContainer Active BEFORE activation: {magicalItemContainer.activeSelf}");
            magicalItemContainer.SetActive(true);
            Debug.Log($"[MagicalItemDisplay] magicalItemContainer Active AFTER activation: {magicalItemContainer.activeSelf}");

            PlayEffects();
            PlayAudio();

            StartCoroutine(WaitAndInvoke(displayDuration, () =>
            {

                Debug.Log($"[MagicalItemDisplay] Invoking onComplete after magical display. magicalItemContainer Active={magicalItemContainer.activeSelf}");
                HideMagicalDisplay();
                onComplete?.Invoke();
                Debug.Log("[MagicalItemDisplay] onComplete callback invoked after displaying magical items.");

            }));
        }
        else
        {
            Debug.Log("[MagicalItemDisplay] Has Displayed block SKIPED");

            Debug.Log("[MagicalItemDisplay] No magical items to display, invoking callback after delay.");
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Hides all magical items and stops effects.
    /// </summary>
    private void HideMagicalDisplay()
    {
        if (magicalItemContainer != null)
        {
            Debug.Log($"[MagicalItemDisplay] Hiding magicalItemContainer. Active before hide: {magicalItemContainer.activeSelf}");
            magicalItemContainer.SetActive(false);
            Debug.Log($"[MagicalItemDisplay] magicalItemContainer hidden. Active after hide: {magicalItemContainer.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[MagicalItemDisplay] Magical item container is null!");
        }
        if (magicalItemEffect != null && magicalItemEffect.isPlaying) magicalItemEffect.Stop();
    }

    /// <summary>
    /// Plays the magical item effects.
    /// </summary>
    private void PlayEffects()
    {
        if (magicalItemEffect != null)
        {
            magicalItemEffect.Play();
            Debug.Log("[MagicalItemDisplay] Magical item effect started.");
        }
        else
        {
            Debug.LogWarning("[MagicalItemDisplay] Particle effect is not assigned.");
        }
    }
    /// <summary>
    /// Plays the magical item audio.
    /// </summary>
    private void PlayAudio()
    {
        if (magicalItemAudio != null)
        {
            audioSource.clip = magicalItemAudio;
            audioSource.Play();
            Debug.Log("[MagicalItemDisplay] Magical item audio played.");
        }
        else
        {
            Debug.LogWarning("[MagicalItemDisplay] Audio clip is not assigned.");
        }
    }

    private IEnumerator WaitAndInvoke(float duration, Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }
}
