using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement;    // Movement action
        [SerializeField] private InputActionReference jump;        // Jump action
        [SerializeField] private InputActionReference crouch;      // Crouch action
        [SerializeField] private InputActionReference run;         // Run action

        [SerializeField] private float runMultiplier = 1.5f;       // Multiplier for running speed
        [SerializeField] private float inputThreshold = 0.1f;      // Threshold for detecting input values

        private ThirdPersonCharacter m_Character;                  // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                                   // A reference to the main camera in the scene's transform
        private Vector3 m_CamForward;                              // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                                       // The world-relative desired move direction

        private void Awake()
        {
            // Ensure actions are not null
            if (movement == null || jump == null || crouch == null || run == null)
            {
                Debug.LogError("InputActionReferences are not set in the inspector.");
                return;
            }

            // Subscribe to the jump action
            jump.action.performed += ctx => m_Jump = true;
        }

        private void OnEnable()
        {
            // Enable the input actions
            movement.action.Enable();
            jump.action.Enable();
            crouch.action.Enable();
            run.action.Enable();
        }

        private void OnDisable()
        {
            // Disable the input actions
            movement.action.Disable();
            jump.action.Disable();
            crouch.action.Disable();
            run.action.Disable();
        }

        private void Start()
        {
            // Get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            }

            // Get the third person character (this should never be null due to require component)
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (movement == null || jump == null || crouch == null || run == null) return;

            // Read inputs
            Vector2 moveInput = movement.action.ReadValue<Vector2>();
            bool crouchInput = crouch.action.ReadValue<float>() > inputThreshold;
            bool runInput = run.action.ReadValue<float>() > inputThreshold;

            // Calculate camera-relative direction to move:
            if (m_Cam != null)
            {
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = moveInput.y * m_CamForward + moveInput.x * m_Cam.right;
            }
            else
            {
                m_Move = moveInput.y * Vector3.forward + moveInput.x * Vector3.right;
            }

            if (runInput)
            {
                m_Move *= runMultiplier; // Increase speed when running
            }

             // Check if both Space and 'S' key are pressed for a special jump
    if (jump.action.triggered && Keyboard.current.sKey.isPressed)
    {
        m_Character.Move(m_Move, crouchInput, true, true); // Perform the special jump
    }
    else
    {
        m_Character.Move(m_Move, crouchInput, jump.action.triggered);
    }
        }


    }
}