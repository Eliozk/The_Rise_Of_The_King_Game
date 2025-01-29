using UnityEngine;
using TMPro;  // Include the TextMeshPro namespace for text handling

public class itemsScript : MonoBehaviour
{
    // Reference to the TextMeshProUGUI component that will display the item count
    public TextMeshProUGUI itemCounterText;
    // The total number of items required for the player to collect
    public int totalItems = 1; // Default value is 1
    // The number of items the player has already collected
    public int collectedItems = 0;
    private const int ZERO = 0;

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the item counter UI with the current count of collected items
        UpdateCounter();
    }

    // This method is called when the player collects an item
    public void CollectItem()
    {
        // Increment the number of collected items
        collectedItems++;
        // Update the item counter display after the collection
        UpdateCounter();
    }

    // This method updates the item count display on the UI
    private void UpdateCounter()
    {
        // Calculate how many items are remaining to be collected
        int remaining = totalItems - collectedItems;
        // Update the item counter text with the remaining items
        itemCounterText.text = "פריטים שנותרו לאסוף לפתיחת יציאת המבוך: " + remaining;
        // If all items have been collected, update the text to notify the player
        if (remaining <= ZERO)
        {
            itemCounterText.text = "כל הפריטים נאספו! כעת תוכל לצאת מהמבוך ולנצח!!!";
        }
    }

    // This method returns whether the player has collected all the required items
    public bool AllItemsCollected()
    {
        // Return true if the player has collected at least the total number of items
        return collectedItems >= totalItems;
    }
}
