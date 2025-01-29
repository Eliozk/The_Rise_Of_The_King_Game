using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FallingCharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 7f; // Movement speed
    [SerializeField] private float moveDistance = 5f; // Default move distance
    [SerializeField] private float moveMultiplier = 3f; // Multiplier for keyboard movement distance
    [SerializeField] private string characterTag; // Tag of the character ("King" or "Robber")
    private bool hasDecided = false; // Flag to check if a decision has been made
    private Vector3 targetPosition; // Target position for movement
    private bool isMoving = false; // Flag to check if the character is currently moving
    private int two = 2; // Used to divide screen width for determining movement direction
    private float dis = 0.01f; // Distance threshold for stopping movement
    private PlayerInput playerInput; // Reference to Player Input
    private Camera mainCamera; // Reference to the main camera

    // Reference to ScoreManagerInvaders
    private ScoreManagerInvaders scoreManager;
    // Reference to GameManager
    private GameManager gameManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Ensure the scene has a Camera tagged as MainCamera.");
        }

        // Find ScoreManagerInvaders in the scene
        scoreManager = Object.FindFirstObjectByType<ScoreManagerInvaders>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManagerInvaders could not be found in the scene!");
        }
        // Find GameManager in the scene
        gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager could not be found in the scene!");
        }
    }

    private void OnEnable()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("PlayerInput or actions are not set. Ensure the PlayerInput component is configured properly.");
            return;
        }

        // Register input actions
        playerInput.actions["MoveLeft"].performed += MoveLeft;
        playerInput.actions["MoveRight"].performed += MoveRight;
        playerInput.actions["MouseLeft"].performed += OnMouseLeftClick;
        playerInput.actions["MouseRight"].performed += OnMouseRightClick;
    }

    private void OnDisable()
    {
        // Unregister input actions
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions["MoveLeft"].performed -= MoveLeft;
            playerInput.actions["MoveRight"].performed -= MoveRight;
            playerInput.actions["MouseLeft"].performed -= OnMouseLeftClick;
            playerInput.actions["MouseRight"].performed -= OnMouseRightClick;
        }
    }

    private void Update()
    {
        // Check for touchscreen input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleTouch(touchPosition);
        }

        // Smooth movement using Lerp
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, targetPosition) < dis) // Threshold for stopping movement
            {
                isMoving = false; // Stop movement
                hasDecided = true; // Set decision flag
            }
        }
    }

    private void MoveLeft(InputAction.CallbackContext context)
    {
        if (!hasDecided)
        {
            MoveToSide(Vector3.left); // Move to the left
        }
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        if (!hasDecided)
        {
            MoveToSide(Vector3.right); // Move to the right
        }
    }

    private void OnMouseLeftClick(InputAction.CallbackContext context)
    {
        if (!hasDecided && mainCamera != null)
        {
            // Get mouse click position
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            HandleTouch(mousePosition);
        }
    }

    private void OnMouseRightClick(InputAction.CallbackContext context)
    {
        if (!hasDecided && mainCamera != null)
        {
            // Get mouse click position
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            HandleTouch(mousePosition, true);
        }
    }

    private void HandleTouch(Vector2 screenPosition, bool isRightClick = false)
    {
        if (!hasDecided && mainCamera != null)
        {
            // Convert screen position to world position
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));

            // Determine target position based on click or touch
            if (isRightClick || screenPosition.x > Screen.width / two) // Right side
            {
                targetPosition = new Vector3(worldPosition.x + moveDistance, transform.position.y, transform.position.z);
            }
            else // Left side
            {
                targetPosition = new Vector3(worldPosition.x - moveDistance, transform.position.y, transform.position.z);
            }

            isMoving = true; // Start movement
        }
    }

    private void MoveToSide(Vector3 direction)
    {
        // Calculate new target position
        targetPosition = transform.position + direction * moveDistance * moveMultiplier;

        // Start movement
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Triggered with {collision.tag} by {characterTag}");

        // Handle interaction with KingdomZone
        if (collision.CompareTag("KingdomZone"))
        {
            if (characterTag == "Robber")
            {
                Debug.Log("Robber entered the Kingdom! Losing a life.");
                scoreManager?.SubtractScore();

                if (gameManager != null)
                {
                    gameManager.LoseLife();
                }
            }
            else if (characterTag == "King")
            {
                Debug.Log("King entered the Kingdom! All good.");
                scoreManager?.AddScore();
            }
        }
        // Handle interaction with DesertZone
        else if (collision.CompareTag("DesertZone"))
        {
            if (characterTag == "King")
            {
                Debug.Log("King sent to the Desert! Losing a life.");
                scoreManager?.SubtractScore();
                if (gameManager != null)
                {
                    gameManager.LoseLife();

                }
            }
            else if (characterTag == "Robber")
            {
                Debug.Log("Robber sent to the Desert! All good.");
                scoreManager?.AddScore();
            }
        }

        // Destroy the character after reaching the zone
        Destroy(gameObject);
    }
}
