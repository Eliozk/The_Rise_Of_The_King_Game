using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public static CharacterSpawner Instance; // Singleton instance for the CharacterSpawner
    [SerializeField] private GameObject[] characters; // Array of characters to spawn
    [SerializeField] private Transform spawnPoint; // Spawn location in the game world
    [SerializeField] private float spawnInterval = 5f; // Time interval between spawning characters
    private bool isSpawning = false; // Flag to check if spawning is active
    private int zero = 0;
    private float characterUpScale = 1.3f;

    private void Awake()
    {
        // Ensure there's only one instance of the CharacterSpawner
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        StartSpawning(); // Begin spawning characters when the game starts
    }

    private void SpawnCharacter()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is missing! Attempting to reassign...");
            spawnPoint = GameObject.Find("SpawnPoint")?.transform;

            if (spawnPoint == null)
            {
                Debug.LogError("Failed to find or reassign spawn point. Skipping character spawn.");
                return;
            }
        }

        // Select a random character from the array to spawn
        int index = Random.Range(zero, characters.Length);

        // Define the spawn position for the character
        Vector3 spawnPos = new Vector3(zero, spawnPoint.position.y, zero);

        // Instantiate the character at the spawn position with no rotation
        GameObject spawnedCharacter = Instantiate(characters[index], spawnPos, Quaternion.identity);

        // Scale the spawned character by 1.3x
        spawnedCharacter.transform.localScale *= characterUpScale;
    }

    public void StartSpawning()
    {
        // Begin spawning if not already active
        if (!isSpawning)
        {
            isSpawning = true;
            // Schedule the SpawnCharacter method to be called repeatedly
            InvokeRepeating(nameof(SpawnCharacter), zero, spawnInterval);
        }
    }

    public void StopSpawning()
    {
        // Stop spawning if it is currently active
        if (isSpawning)
        {
            isSpawning = false;

            // Cancel the scheduled SpawnCharacter method calls
            CancelInvoke(nameof(SpawnCharacter));
        }
    }
}
