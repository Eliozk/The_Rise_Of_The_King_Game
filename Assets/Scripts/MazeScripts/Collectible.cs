using UnityEngine;
using UnityEngine.Audio;

public class Collectible : MonoBehaviour
{
    // Script to handle item collection
    [SerializeField] private MazeProgress mazeProgress;



   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check for itemsScript
            var itemScript = Object.FindFirstObjectByType<itemsScript>();
            if (itemScript != null)
            {
                itemScript.CollectItem();
            }
            else
            {
                Debug.LogError("[Collectible] itemsScript not found in the scene.");
            }

            // Check for MazeProgress
           
            if (mazeProgress != null)
            {
                mazeProgress.OnItemCollected();
            }
            else
            {
                Debug.LogError("[Collectible] MazeProgress not found in the scene.");
            }

            // Destroy the collectible object
            Destroy(gameObject);
        }
    }

}
