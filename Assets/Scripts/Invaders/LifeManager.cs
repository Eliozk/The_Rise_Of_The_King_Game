using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance; // Singleton instance of the LifeManager
    [SerializeField] private Image[] hearts; // UI elements representing the player's hearts (lives)

    private void Awake()
    {
        // Ensure only one instance of LifeManager exists
        if (Instance == null)
        {
            Instance = this; // Set the instance to this object
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate LifeManager objects
        }
    }

    
    /// Updates the heart UI to reflect the current number of lives.
    
    public void UpdateHearts(int livesRemaining)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < livesRemaining; // Enable hearts if the index is within the range of remaining lives
        }
    }

   
    /// Resets all hearts to be visible (enabled).
    
    public void ResetHearts()
    {
        foreach (Image heart in hearts)
        {
            heart.enabled = true; // Enable all hearts to reset lives UI
        }
    }
}
