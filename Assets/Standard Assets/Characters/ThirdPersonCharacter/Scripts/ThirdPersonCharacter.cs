using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    // This script controls the third-person character's movement, animation, and physics interactions.

    [RequireComponent(typeof(Rigidbody))] // Ensures a Rigidbody component is attached to the GameObject.
    [RequireComponent(typeof(CapsuleCollider))] // Ensures a CapsuleCollider component is attached to the GameObject.
    [RequireComponent(typeof(Animator))] // Ensures an Animator component is attached to the GameObject.
    public class ThirdPersonCharacter : MonoBehaviour
    {
        // Public variables exposed in the Unity Inspector for fine-tuning character behavior.
        [SerializeField] float m_MovingTurnSpeed = 360; // Turn speed while moving.
        [SerializeField] float m_StationaryTurnSpeed = 180; // Turn speed while stationary.
        [SerializeField] float m_JumpPower = 12f; // Vertical jump force.

        const float m_SpecialJumpPower = 12f;  // Special jump force for the cheat code

        [Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f; // Multiplier to increase gravity effect for realistic falls.
        [SerializeField] float m_RunCycleLegOffset = 0.2f; // Offset for running animation cycle to synchronize leg movement.
        [SerializeField] float m_MoveSpeedMultiplier = 1f; // Multiplier for character movement speed.
        [SerializeField] float m_AnimSpeedMultiplier = 1f; // Multiplier for animation playback speed.
        [SerializeField] float m_GroundCheckDistance = 0.1f; // Distance used to check if the character is grounded.

        // Internal variables for managing components and state.
        Rigidbody m_Rigidbody; // Reference to the Rigidbody component.
        Animator m_Animator; // Reference to the Animator component.
        bool m_IsGrounded; // Tracks if the character is on the ground.
        float m_OrigGroundCheckDistance; // Stores the original ground check distance.
        const float k_Half = 0.5f; // Constant for dividing values by 2.
        float m_TurnAmount; // Amount to turn the character.
        float m_ForwardAmount; // Amount to move forward.
        Vector3 m_GroundNormal; // Normal vector of the ground surface.
        float m_CapsuleHeight; // Original height of the CapsuleCollider.
        Vector3 m_CapsuleCenter; // Original center of the CapsuleCollider.
        CapsuleCollider m_Capsule; // Reference to the CapsuleCollider component.
        bool m_Crouching; // Tracks if the character is crouching.

        //consts
        private const float one = 1f;
        private const float zero = 0;
        private const int ONE = 1;

        void Start()
        {
            // Initialize component references and setup default values.
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;

            // Freeze rotations to prevent unintended physics interactions.
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        // Handles character movement, crouching, and jumping.
        public void Move(Vector3 move, bool crouch, bool jump)
        {
            if (move.magnitude > one) move.Normalize(); // Normalize movement vector to maintain consistent speed.
            move = transform.InverseTransformDirection(move); // Convert movement to local space.
            CheckGroundStatus(); // Determine if the character is grounded.
            move = Vector3.ProjectOnPlane(move, m_GroundNormal); // Adjust movement to align with the ground's surface.
            m_TurnAmount = Mathf.Atan2(move.x, move.z); // Calculate turn amount based on movement direction.
            m_ForwardAmount = move.z; // Forward movement value.

            ApplyExtraTurnRotation(); // Apply additional rotation for smoother turning.

            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump); // Manage movement while on the ground.
            }
            else
            {
                HandleAirborneMovement(); // Manage movement while airborne.
            }

            ScaleCapsuleForCrouching(crouch); // Adjust CapsuleCollider size for crouching.
            PreventStandingInLowHeadroom(); // Prevent standing up in areas with low ceilings.

            UpdateAnimator(move); // Update animation parameters.
        }

        // Adjusts the CapsuleCollider size when crouching.
        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return; // Exit if already crouching.
                m_Capsule.height = m_Capsule.height / 2f; // Halve the height of the collider.
                m_Capsule.center = m_Capsule.center / 2f; // Adjust the center accordingly.
                m_Crouching = true; // Mark as crouching.
            }
            else
            {
                // Check if there's enough headroom to stand up.
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true; // Stay crouched if there�s an obstacle above.
                    return;
                }
                m_Capsule.height = m_CapsuleHeight; // Restore the original height.
                m_Capsule.center = m_CapsuleCenter; // Restore the original center.
                m_Crouching = false; // Mark as not crouching.
            }
        }

        // Prevents the character from standing up in areas with low ceilings.
        void PreventStandingInLowHeadroom()
        {
            if (!m_Crouching)
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true; // Enforce crouching if there's not enough headroom.
                }
            }
        }

        // Updates animation parameters based on movement and state.
        void UpdateAnimator(Vector3 move)
        {
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime); // Set forward movement value.
            m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime); // Set turning value.
            m_Animator.SetBool("Crouch", m_Crouching); // Set crouching state.
            m_Animator.SetBool("OnGround", m_IsGrounded); // Set grounded state.

            if (!m_IsGrounded)
            {
                m_Animator.SetFloat("Jump", m_Rigidbody.linearVelocity.y); // Update jump animation based on vertical velocity.
            }

            // Adjust animation speed based on movement and grounded state.
            if (m_IsGrounded && move.magnitude > zero)
            {
                m_Animator.speed = m_AnimSpeedMultiplier; // Use custom speed multiplier.
            }
            else
            {
                m_Animator.speed = ONE; // Default speed while airborne or idle.
            }
        }

        // Applies extra gravity force for more realistic airborne behavior.
        void HandleAirborneMovement()
        {
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.linearVelocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f; // Adjust ground check sensitivity.
        }

        // Handles grounded movement, including jumping.
        // void HandleGroundedMovement(bool crouch, bool jump)
        // {
        //     if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        //     {
        //         m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, m_JumpPower, m_Rigidbody.linearVelocity.z); // Apply jump force.
        //         m_IsGrounded = false; // Mark as airborne.
        //         m_Animator.applyRootMotion = false; // Disable root motion during jump.
        //         m_GroundCheckDistance = 0.1f; // Reset ground check distance.
        //     }
        // }
  public void Move(Vector3 move, bool crouch, bool jump, bool specialJump = false)
{
    if (move.magnitude > 1) move.Normalize();  // Normalize movement vector
    move = transform.InverseTransformDirection(move);
    CheckGroundStatus();
    move = Vector3.ProjectOnPlane(move, m_GroundNormal);
    m_TurnAmount = Mathf.Atan2(move.x, move.z);
    m_ForwardAmount = move.z;

    ApplyExtraTurnRotation();

    if (m_IsGrounded)
    {
        if (specialJump)
        {
            m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, m_SpecialJumpPower, m_Rigidbody.linearVelocity.z); // Apply special jump force
        }
        else
        {
            HandleGroundedMovement(crouch, jump);
        }
    }
    else
    {
        HandleAirborneMovement();
    }

    ScaleCapsuleForCrouching(crouch);
    PreventStandingInLowHeadroom();

    UpdateAnimator(move);
}


void HandleGroundedMovement(bool crouch, bool jump)
{
    if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
    {
        m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, m_JumpPower, m_Rigidbody.linearVelocity.z); // Apply jump force.
        m_IsGrounded = false; // Mark as airborne.
        m_Animator.applyRootMotion = false; // Disable root motion during jump.
        m_GroundCheckDistance = 0.1f; // Reset ground check distance.
    }
}


        // Adds extra rotation for smoother turning.
        void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        // Adjusts root motion velocity for grounded movement.
        public void OnAnimatorMove()
        {
            if (m_IsGrounded && Time.deltaTime > zero)
            {
                Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
                v.y = m_Rigidbody.linearVelocity.y; // Preserve vertical velocity.
                m_Rigidbody.linearVelocity = v;
            }
        }

    
        // Checks if the character is grounded and updates related state.
        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // Visualize the ground check ray in the Scene view for debugging.
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal; // Store the ground's normal vector.
                m_IsGrounded = true; // Mark as grounded.
                m_Animator.applyRootMotion = true; // Enable root motion.
            }
            else
            {
                m_IsGrounded = false; // Mark as airborne.
                m_GroundNormal = Vector3.up; // Default ground normal to up direction.
                m_Animator.applyRootMotion = false; // Disable root motion.
            }
        }
    }

}
