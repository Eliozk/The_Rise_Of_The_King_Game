using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles drag-and-drop functionality for objects, allowing snapping to specific slots.
/// Supports RectTransform slots and ensures proper positioning.
/// </summary>
public class DragAndDrop : MonoBehaviour
{
    // Stores the original position and scale of the object
    private Vector3 startLocalPosition;
    private Vector3 originalScale;
    private int originalSortingOrder;

    [Header("Scaling Settings")]
    [Tooltip("Scale factor applied when the object is dragged.")]
    public float scaleFactor;

    [Tooltip("Object scale when it snaps into the correct slot.")]
    public Vector3 slotScale;

    [Header("Snapping Settings")]
    [Tooltip("Maximum distance allowed for snapping to the correct slot.")]
    public float snapDistance = 50f;

    // Flags for tracking object state
    private bool isInCorrectSlot = false;
    private bool isDragging = false;
    private bool isTouchingSlot = false;

    [Header("Slot Settings")]
    [Tooltip("The correct slot where this object should snap.")]
    public RectTransform correctSlot; // Changed to RectTransform for UI compatibility

    [Header("Progress Bar Reference")]
    [Tooltip("Reference to the ProgressBarManager for progress tracking.")]
    public ProgressBarManager progressBarManager;

    private SpriteRenderer spriteRenderer;

    [Header("Input System")]
    [Tooltip("Reference to the Drag Input Action.")]
    public InputActionAsset inputActions;
    private InputAction dragAction;

    /// <summary>
    /// Initializes object properties, sets up input handling, and stores initial position.
    /// </summary>
    void Start()
    {
        Debug.Log("[DragAndDrop] Initializing Drag-and-Drop System...");

        if (progressBarManager != null)
        {
            progressBarManager.InitializeProgressBar(progressBarManager.progressBarStages.Length);
            Debug.Log("[DragAndDrop] ProgressBarManager initialized successfully.");
        }
        else
        {
            Debug.LogError("[DragAndDrop] Missing reference to ProgressBarManager!");
        }

        startLocalPosition = transform.localPosition;
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
        dragAction = inputActions.FindAction("Drag");
        dragAction.Enable();
    }

    /// <summary>
    /// Updates the object position while dragging.
    /// Converts screen coordinates to world position.
    /// </summary>
    void Update()
    {
        if (isDragging)
        {
            Vector2 pointerPosition = dragAction.ReadValue<Vector2>();

            // Ensure pointer position is within screen bounds
            pointerPosition.x = Mathf.Clamp(pointerPosition.x, 10, Screen.width - 10);
            pointerPosition.y = Mathf.Clamp(pointerPosition.y, 10, Screen.height - 10);

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(pointerPosition.x, pointerPosition.y, Mathf.Abs(Camera.main.transform.position.z))
            );

            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }
    }

    /// <summary>
    /// Handles the start of a drag operation.
    /// Adjusts scale and sorting order.
    /// </summary>
    private void OnMouseDown()
    {
        if (!isInCorrectSlot)
        {
            isDragging = true;

            // Enlarge object while dragging
            transform.localScale = originalScale * scaleFactor;

            // Ensure object appears above others
            spriteRenderer.sortingOrder = 2;

            Debug.Log("[DragAndDrop] Started dragging.");
        }
    }

    /// <summary>
    /// Handles the release of the object.
    /// If placed correctly, snaps to the slot and updates progress.
    /// </summary>
    private void OnMouseUp()
    {
        if (!isInCorrectSlot)
        {
            isDragging = false;

            if (isTouchingSlot && Vector3.Distance(transform.position, correctSlot.position) < snapDistance)
            {
                RectTransform slotRectTransform = correctSlot.GetComponent<RectTransform>();
                if (slotRectTransform != null)
                {
                    // Convert slot position to world space and set object position
                    Vector3 worldSlotPosition = slotRectTransform.position;
                    transform.position = worldSlotPosition;
                }
                else
                {
                    transform.position = correctSlot.position;
                }

                transform.localScale = slotScale;
                isInCorrectSlot = true;
                progressBarManager.AddProgress();

                Debug.Log("[DragAndDrop] Object snapped to correct slot.");
            }
            else
            {
                // Reset to original position if not correctly placed
                transform.localPosition = startLocalPosition;
                transform.localScale = originalScale;
            }

            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    /// <summary>
    /// Detects when the object enters the correct slot's trigger collider.
    /// </summary>
    /// <param name="other">The collider the object interacts with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == correctSlot.gameObject)
        {
            isTouchingSlot = true;
            Debug.Log($"[DragAndDrop] Entered correct slot: {other.gameObject.name}");
        }
    }

    /// <summary>
    /// Detects when the object exits the correct slot's trigger collider.
    /// </summary>
    /// <param name="other">The collider the object interacts with.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == correctSlot.gameObject)
        {
            isTouchingSlot = false;
            Debug.Log($"[DragAndDrop] Exited correct slot: {other.gameObject.name}");
        }
    }
}
