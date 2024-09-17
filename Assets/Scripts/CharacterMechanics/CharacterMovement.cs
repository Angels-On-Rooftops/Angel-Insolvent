using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum MovementState
{
    Grounded,
    Airbourne,
}

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    /**         START USER PROPERTIES           **/

    [SerializeField]
    [Tooltip("The camera that the character's movement should be based off of. " +
        "Player movement is relative to this camera.")]
    public Camera Camera;

    [Space(20)]

    [SerializeField]
    [Tooltip("The keybinds that control lateral character movement. To restrict character movement to a single axis, unbind the directions you don't want accessible.")]
    InputAction Walk;

    [SerializeField]
    [Min(0)]
    [Tooltip("The speed the character moves at in units/second.")]
    public float WalkSpeed = 16;

    [SerializeField]
    [Min(0)]
    [Tooltip("Determines how fast the character falls relative to everything else in the scene. " +
        "A higher multiplier will result in a character that lifts off faster and falls faster.")]
    float GravityMultiplier = 8;

    [Space(10)]

    [SerializeField]
    [Tooltip("The keybinds that make the character jump.")]
    InputAction Jump;

    [SerializeField]
    [Tooltip("The maximum amount of jumps the character has before they touch the ground again.")]
    int Jumps = 1;

    [SerializeField]
    [Min(0)]
    [Tooltip("The maximum height of the character's jump in units. If gravity is changed, " +
        "the initial velocity of the jump will change accordingly to give the same jump height.")]
    float JumpHeight = 3;

    [SerializeField]
    [Tooltip("On subsequent midair jumps, the jump height will increment based off of this property.\n\n" +
        "For example, if the initial jump height is 5, the bonus is 5, and the number of jumps the character has is 3: " +
        "On the first jump the character will jump 5 units, then on the double jump it will jump 10, then on the triple jump it will jump 15.")]
    float JumpHeightBonus = 0;

    [SerializeField]
    [Tooltip("After pressing the jump key in mid air, the character will jump immediately " +
        "if they hit the ground within this timeframe (in seconds).")]
    float JumpBufferTime = 0.1f;

    [SerializeField]
    [Tooltip("After leaving a ledge, if the player presses the jump key within " +
        "this timeframe, they will jump in midair. This is useful for when the " +
        "player wants to jump at the edge of a platform.")]
    float CoyoteTime = 0.1f;

    [SerializeField]
    [Tooltip("The maximum speed the character can fall at.")]
    float DownwardTerminalVelocity = 64;

    [Space(10)]

    [SerializeField]
    [Tooltip("If enabled, to keep characters from floating when moving off ledges, they will snap " +
        "downwards when the ground slope changes.")]
    bool SnapToGround = true;

    [SerializeField]
    [Min(0)]
    [Tooltip("Determines the maximum distance the controller will check " +
        "for ground below the character before snapping it downward if SnapToGround is enabled.")]
    float MaximumSnappingDistance = 0.2f;

    /**         END USER PROPERTIES                 **/

    /**         START EVENTS                        **/

    public event Action StartedWalking;
    public event Action StoppedWalking;
    public event Action<int> Jumped;
    public event Action JumpRequested;
    public event Action Landed;

    /**         END EVENTS                          **/

    /**         START CALCULATED PROPERTIES         **/

    float JumpPower
    {
        get
        {
            float height = JumpHeight + (Jumps - JumpsRemaining) * JumpHeightBonus;
            return Mathf.Sqrt(2 * CharacterGravity() * height);
        }
    }

    /**         END CALCULATED PROPERTIES           **/

    [NonSerialized]
    public Vector3 GravityUpDirection = new(0, 1, 0);
    
    [NonSerialized]
    public float VerticalSpeed = 0;

    Vector3 MovementDirection = new();
    float DefaultWalkSpeed;

    MovementState State = MovementState.Airbourne;
    bool IsJumping = false;
    int JumpsRemaining;
    float LastTimeGrounded = 0;
    float dx = 0.01f;
    CharacterController controller;

    // the last platform the character landed on
    GameObject LastPlatform;
    // the position of the last platform on the previous frame
    Vector3 LastPlatformLastPosition;
    // the velocity of the last platform on the previous frame
    Vector3 LastPlatformLastVelocity;

    bool GravityEnabled = true;
    bool StateChangeEnabled = true;
    bool LateralMovementEnabled = true;
    bool RotationEnabled = true;

    void Awake()
    {
        DefaultWalkSpeed = WalkSpeed;
        controller = GetComponent<CharacterController>();
        JumpsRemaining = Jumps;
    }

    void OnEnable()
    {
        Walk.performed += DoWalk;
        Jump.performed += DoJumpInput;

        Walk.Enable();
        Jump.Enable();

        GravityEnabled = true;
        StateChangeEnabled = true;
        LateralMovementEnabled = true;
        RotationEnabled = true;
    }

    private void OnDisable()
    {
        Walk.performed -= DoWalk;
        Jump.performed -= DoJumpInput;

        Walk.Disable();
        Jump.Disable();

        GravityEnabled = false;
        StateChangeEnabled = false;
        LateralMovementEnabled = false;
        RotationEnabled = false;
    }

    void DoWalk(CallbackContext c)
    {
        Vector3 oldMovementDirection = MovementDirection;

        Vector2 move2d = c.ReadValue<Vector2>();
        MovementDirection = new Vector3(move2d.x, 0, move2d.y);

        if (oldMovementDirection.magnitude == 0 && MovementDirection.magnitude != 0)
        {
            StartedWalking?.Invoke();
        }

        if (MovementDirection.magnitude == 0 && oldMovementDirection.magnitude != 0)
        {
            StoppedWalking?.Invoke();
        }
    }

    bool DoJump(bool isFromBuffer)
    {
        if (JumpsRemaining > 1 || (TimeSinceGrounded() < CoyoteTime && !IsJumping))
        {
            VerticalSpeed = JumpPower;

            State = MovementState.Airbourne;
            IsJumping = true;
            JumpsRemaining--;

            Jumped?.Invoke(Jumps - JumpsRemaining);

            return true;
        }
        else if (!isFromBuffer)
        {
            StartCoroutine(Macros.Buffer(JumpBufferTime, () => DoJump(true)));
            return false;
        }

        return false;
    }

    void DoJumpInput(CallbackContext _) {
        JumpRequested?.Invoke();
        DoJump(false);
    }



    Vector3 ForwardMovementDirectionFromCamera()
    {
        Vector3 forward;

        if (Camera.TryGetComponent(out CharacterCamera PlayerCamera) && PlayerCamera.enabled)
        {
            forward = PlayerCamera.GetNextCameraTransform().forward;
        }
        else
        {
            forward = Camera.transform.forward;
        }

        return Vector3.Scale(-forward, Vector3.one - Vector3.up);
    }

    Vector3 MovementVelocity(Vector3 forwardVector)
    {
        return Quaternion.LookRotation(-forwardVector, Vector3.up) * MovementDirection * WalkSpeed;
    }

    Vector3 FacingDirection()
    {
        Vector3 moveVelocity = MovementVelocity(ForwardMovementDirectionFromCamera());

        return moveVelocity.magnitude != 0 ? Vector3.Normalize(moveVelocity) : transform.forward;
    }
    
    float CharacterGravity()
    {
        return Physics.gravity.magnitude * GravityMultiplier;
    }

    public Vector3 TopSphereCenter()
    {
        return transform.position + CharacterUpVector() * (controller.height / 2 - controller.radius);
    }

    public Vector3 BottomSphereCenter()
    {
        return transform.position - CharacterUpVector() * (controller.height / 2 - controller.radius);
    }

    Vector3 FootPosition()
    {
        return transform.position - CharacterUpVector() * (controller.height / 2 + controller.skinWidth);
    }

    public Vector3 CharacterUpVector()
    {
        return GravityUpDirection;
    }

    bool GroundHitInfo(float checkDistance, out RaycastHit hit)
    {
        return Physics.CapsuleCast(
            BottomSphereCenter(), TopSphereCenter(), controller.radius-dx, -CharacterUpVector(),
            out hit, checkDistance,
            ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
        );
    }

    Vector3? GroundNormal(float checkDistance)
    {
        bool didHit = GroundHitInfo(checkDistance, out RaycastHit hit);

        return didHit switch 
        {
            false => null,
            _ => hit.normal,
        };
    }

    float GroundAngle(float checkDistance)
    {
        Vector3? normal = GroundNormal(checkDistance);

        return normal switch
        {
            null => 90,
            _ => Vector3.Angle((Vector3)normal, GravityUpDirection),
        };
    }

    public bool IsOnStableGround()
    {
        if (GroundAngle(controller.height/2f + dx) > controller.slopeLimit)
        {
            return false;
        }

        return IsOnGround();
    }

    public bool IsOnGround()
    {
        bool didHit = Physics.CheckSphere(
            BottomSphereCenter() - CharacterUpVector() * (/*2 * dx +*/ controller.skinWidth + dx),
            controller.radius,
            ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
        );

        return didHit;
    }

    bool IsHittingHead()
    {
        return Mathf.Abs(Vector3.Dot(controller.velocity, CharacterUpVector())) < dx && VerticalSpeed > dx;
    }

    public bool IsFalling()
    {
        return VerticalSpeed < 0;
    }

    float TimeSinceGrounded()
    {
        return Time.time - LastTimeGrounded;
    }

    void ApplyGravity()
    {
        if (IsOnStableGround() && !IsJumping)
        {
            VerticalSpeed = Mathf.Max(0, VerticalSpeed);
            return;
        }

        VerticalSpeed -= CharacterGravity() * Time.deltaTime;
    }

    void ApplyMovementVelocity()
    {
        Vector3 forwardFromCamera = ForwardMovementDirectionFromCamera();
        Vector3 moveVelocity = MovementVelocity(forwardFromCamera);
        Vector3 verticalVelocity = CharacterUpVector() * VerticalSpeed;

        if (IsOnGround() && !IsOnStableGround())
        {
            bool didHit = GroundHitInfo(controller.height/2f, out RaycastHit hit);
            
            if (didHit)
            {
                Physics.Raycast(
                    transform.position, hit.point - transform.position, out RaycastHit hit2, controller.height, ControlConstants.RAYCAST_MASK
                );

                if (Vector3.Dot(hit2.normal, CharacterUpVector()) < 1-dx && Vector3.Dot(hit2.normal, CharacterUpVector()) > dx)
                {
                    moveVelocity = Vector3.Cross(Vector3.Cross(CharacterUpVector(), (Vector3)hit2.normal), (Vector3)hit2.normal) * -VerticalSpeed;
                }
            }
        }

        Vector3 combinedVelocity = verticalVelocity + moveVelocity;

        if (IsJumping || !IsOnStableGround() || VerticalSpeed > dx)
        {
            controller.Move(combinedVelocity * Time.deltaTime);
            return;
        }

        Vector3 normal = GroundNormal(controller.height).GetValueOrDefault(CharacterUpVector());

        Vector3 velocityParallelToPlane = Vector3.Normalize(Vector3.ProjectOnPlane(combinedVelocity, normal)) * combinedVelocity.magnitude;

        controller.Move(velocityParallelToPlane * Time.deltaTime);
    }

    void UpdateState()
    {
        bool isGroundWithinSnappingDistance = Physics.Raycast(
            transform.position, 
            -CharacterUpVector(), 
            controller.height / 2 + controller.skinWidth + MaximumSnappingDistance, 
            ControlConstants.RAYCAST_MASK);

        if (State == MovementState.Airbourne && IsOnStableGround() && VerticalSpeed <= dx)
        {
            Landed?.Invoke();

            State = MovementState.Grounded;

            IsJumping = false;
            JumpsRemaining = Jumps;

            return;
        }

        if (SnapToGround && State == MovementState.Grounded)
        {
            if (isGroundWithinSnappingDistance && VerticalSpeed <= 0)
            {
                SnapCharacterToGround();
            }
            else
            {
                State = MovementState.Airbourne;
            }

            return;
        }

        if (!IsOnStableGround())
        {
            State = MovementState.Airbourne;
        }
    }

    void ApplyRotation()
    {
        Vector3 characterUp = CharacterUpVector();

        transform.LookAt(transform.position + FacingDirection(), characterUp);
    }

    void UpdateLastGrounded()
    {
        //if (IsOnStableGround())
        if (State == MovementState.Grounded)
        {
            LastTimeGrounded = Time.time;
        }
    }

    void SnapCharacterToGround()
    {
        bool didCapsuleHit = Physics.CapsuleCast(
            TopSphereCenter(), BottomSphereCenter(), controller.radius + controller.skinWidth - dx, 
            -CharacterUpVector(), 
            out RaycastHit capsuleHit, MaximumSnappingDistance, ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore);

        bool isCapsuleOverGeometry = Physics.CheckCapsule(
                TopSphereCenter(), BottomSphereCenter(), controller.radius + controller.skinWidth + dx, ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
            );

        if (didCapsuleHit & !isCapsuleOverGeometry)
        {
            transform.position += capsuleHit.distance * -CharacterUpVector();
        }
    }

    public void OverrideWalkSpeed(float speed)
    {
        WalkSpeed = speed;
    }

    public void ResetWalkSpeed()
    {
        WalkSpeed = DefaultWalkSpeed;
    }

    public void EnableVerticalMovement()
    {
        GravityEnabled = true;
        StateChangeEnabled = true;
    }

    public void DisableVerticalMovement()
    {
        GravityEnabled = false;
        StateChangeEnabled = false;

        State = MovementState.Grounded;
        VerticalSpeed = 0;
        IsJumping = false;
    }

    public void EnableLateralMovement()
    {
        LateralMovementEnabled = true;
    }

    public void DisableLateralMovement()
    {
        LateralMovementEnabled = false;
    }

    public void EnableRotation()
    {
        RotationEnabled = true;
    }

    public void DisableRotation()
    {
        RotationEnabled = false;
    }

    void Update()
    {
        if (GravityEnabled)
        {
            ApplyGravity();
        }

        if (LateralMovementEnabled)
        {
            ApplyMovementVelocity();

            // for transferred momentum:
            // if on same platform, update vars and add dx to char position
            // if on different platform, dont do anything to char position, update vars
            // if in air, dont update vars, but do add the velocity to char
        }

        if (RotationEnabled)
        {
            ApplyRotation();
        }

        if (StateChangeEnabled)
        {
            UpdateState();
        }

        UpdateLastGrounded();

        if (IsHittingHead())
        {
            VerticalSpeed = 0;
        }

        VerticalSpeed = Mathf.Max(VerticalSpeed, -DownwardTerminalVelocity);
    }
}