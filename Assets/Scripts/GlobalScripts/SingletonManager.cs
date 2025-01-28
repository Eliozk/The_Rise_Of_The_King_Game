using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the singleton instance of GameManager.
/// </summary>
public class SingletonManager : MonoBehaviour
{
    public static SingletonManager singltoneInstance { get; private set; }

    public string managedSceneName;

    public ExplanationManager explanationManager;

    [Header("Managed Scenes")]
    [Tooltip("Scenes that this GameManager will manage.")]
    public string[] managedScenes;

    [Header("Scriptable Objects")]
    [Tooltip("Reference to the ScoreData ScriptableObject.")]
    public ScoreData scoreData;

    [Tooltip("Reference to the TimeData ScriptableObject.")]
    public TimeData timeData;

    /// <summary>
    /// Manages the singleton instance of the GameManager for the managed scene.
    /// This method first logs the current and managed scene names.
    /// It then checks if the current scene matches the managed scene name.
    /// If a different instance of the GameManager exists, it destroys the older instance and sets this instance as the new singleton.
    /// Otherwise, it sets this instance as the singleton and ensures it persists across scene loads.
    /// If the scene does not match, it destroys this instance.
    /// </summary>
    private void Awake()
    {
        Debug.Log("Current scene: " + SceneManager.GetActiveScene().name + ", Managed scene: " + managedSceneName);

        if (managedSceneName == SceneManager.GetActiveScene().name)
        {
            if (singltoneInstance != null && singltoneInstance != this)
            {
                Debug.Log($"[Singleton]  destroy:: Awake called for {gameObject.name}. ManagedSceneName: {managedSceneName}, ActiveScene: {SceneManager.GetActiveScene().name}");
                Debug.Log("Destroying old GameManager instance for new scene-specific instance.");
                Destroy(singltoneInstance.gameObject);
            }
            else
            {
                Debug.Log("No existing instance found or current instance is the singleton. Setting as Singleton.");
            }
            singltoneInstance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager set as DontDestroyOnLoad for " + managedSceneName);
            // Initialize other components
            InitializeComponents();
        }
        else
        {
            Debug.Log("This GameManager is not for this scene, destroying.");
            Destroy(gameObject);
        }
    }



    public void InitializeComponents()
    {
        Debug.Log("[SingletonManager] Initializing components...");

        // Initialize ExplanationManager if not already assigned
        if (explanationManager == null)
        {
            explanationManager = GetComponent<ExplanationManager>();
            if (explanationManager == null)
            {
                explanationManager = gameObject.AddComponent<ExplanationManager>();
            }
        }

        // Verify and log the state of ScriptableObjects
        if (scoreData == null)
        {
            Debug.LogError("[SingletonManager] ScoreData ScriptableObject is missing!");
        }
        else
        {
            Debug.Log("[SingletonManager] ScoreData initialized. Total games: " + scoreData.gameScores.Count);
        }

        if (timeData == null)
        {
            Debug.LogError("[SingletonManager] TimeData ScriptableObject is missing!");
        }
        else
        {
            Debug.Log("[SingletonManager] TimeData initialized. Total games: " + timeData.gameTimes.Count);
        }
    }
}
