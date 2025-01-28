using UnityEngine;

public class BlockScript : MonoBehaviour
{
    // Reference to the item manager script
    private itemsScript itemManager;

    // Start is called before the first frame update
    void Start()
    {
        // Find the instance of itemsScript in the scene
        itemManager = FindFirstObjectByType<itemsScript>();
    }


    // Update is called once per frame
    void Update()
    {
        // Check if the itemManager exists and whether all items have been collected
        if (itemManager != null && itemManager.AllItemsCollected())
        {
            // Log a message that the maze is completed
            Debug.Log("you can now exit the maze!");

            // Destroy the blocking object (the wall)
            Destroy(gameObject);
        }
    }


    // This function is called when a collision occurs
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collider involved in the collision is the player
        if (collision.collider.CompareTag("Player"))
        {
            // Check if the itemManager exists and whether all items have been collected
            if (itemManager != null && itemManager.AllItemsCollected())
            {
                // Log a message that the maze is completed
                Debug.Log("Maze Completed!");

                // Destroy the blocking object (the wall) that blocks the player
                Destroy(gameObject);
            }
            else
            {
                // Calculate how many items are remaining to be collected
                int remainingItems = itemManager.totalItems - itemManager.collectedItems;

                // Log a message instructing the player to collect more items
                Debug.Log("Collect " + remainingItems + " more item(s) to unlock the exit!");
            }
        }
    }
}
