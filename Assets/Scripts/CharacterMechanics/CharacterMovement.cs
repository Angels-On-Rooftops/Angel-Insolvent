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
    public float GravityMultiplier = 8;

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
    public float JumpHeight = 3;

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

    /**         END USER PROPERTIES                 **/

    /**         START EVENTS                        **/

    public event Action StartedWalking;
    public event Action StoppedWalking;
    public event Action<int> Jumped;
    public event Action JumpRequested;
    public event Action Landed;

    /**         END EVENTS                          **/



    [NonSerialized]
    public Vector3 GravityUpDirection = new(0, 1, 0);

    [NonSerialized]
    public float VerticalSpeed = 0;

    Vector3 MovementDirection = new();
    MovementState State = MovementState.Airbourne;
    bool IsJumping = false;
    int ExtraJumpsRemaining;
    float LastTimeGrounded = 0;
    float dx = 0.01f;
    CharacterController controller;

    const float SNAPPING_DISTANCE = 1;

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

    /**         START CALCULATED PROPERTIES         **/
    float JumpPower
    {
        get
        {
            float height = JumpHeight + (Jumps - ExtraJumpsRemaining - 1) * JumpHeightBonus;
            return Mathf.Sqrt(2 * CharacterGravity() * height);
        }
    }

    bool IsRising
    {
        get
        {
            return VerticalSpeed > 0;
        }
    }
    /**         END CALCULATED PROPERTIES           **/

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        ExtraJumpsRemaining = Jumps - 1;
    }

    void OnEnable()
    {
        if (Walk is not null)
        {
            Walk.performed += DoWalk;
            Walk.Enable();
        }

        if (Jump is not null)
        {
            Jump.performed += DoJumpInput;
            Jump.Enable();
        }

        GravityEnabled = true;
        StateChangeEnabled = true;
        LateralMovementEnabled = true;
        RotationEnabled = true;
    }

    private void OnDisable()
    {
        if (Walk is not null)
        {
            Walk.performed -= DoWalk;
            Walk.Disable();
        }

        if (Jump is not null)
        {
            Jump.performed -= DoJumpInput;
            Jump.Disable();
        }

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
        bool CanJump = TimeSinceGrounded() <= CoyoteTime && !IsJumping;
        if (!CanJump && ExtraJumpsRemaining > 0)
        {
            ExtraJumpsRemaining--;
            CanJump = true;
        }

        if (CanJump)
        {
            VerticalSpeed = JumpPower;

            State = MovementState.Airbourne;
            IsJumping = true;
            Jumped?.Invoke(Jumps - ExtraJumpsRemaining);

            return true;
        }
        else if (!isFromBuffer)
        {
            StartCoroutine(Macros.Buffer(JumpBufferTime, () => DoJump(true)));
            return false;
        }

        return false;
    }

    void DoJumpInput(CallbackContext _)
    {
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

    public Vector3 CharacterUpVector()
    {
        return GravityUpDirection;
    }

    bool GroundHitInfo(float checkDistance, out RaycastHit hit)
    {
        return Physics.CapsuleCast(
            BottomSphereCenter(), TopSphereCenter(), controller.radius - dx, -CharacterUpVector(),
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
        if (GroundAngle(controller.height / 2f + dx) > controller.slopeLimit)
        {
            return false;
        }

        return IsOnGround();
    }

    public bool IsOnGround()
    {
        bool didHit = Physics.CheckSphere(
            BottomSphereCenter() - CharacterUpVector() * (controller.skinWidth + dx),
            controller.radius,
            ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
        );

        return didHit;
    }

    bool IsHittingHead()
    {
        return Mathf.Abs(Vector3.Dot(controller.velocity, CharacterUpVector())) < dx && IsRising;
    }

    float TimeSinceGrounded()
    {
        if (IsOnStableGround())
        {
            return 0;
        }

        return Time.time - LastTimeGrounded;
    }

    void ApplyGravity()
    {
        if (IsOnStableGround() /*&& !IsJumping*/)
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

        // character is on a slope that's too steep, move them downwards
        if (IsOnGround() && !IsOnStableGround() && GroundHitInfo(controller.height / 2f, out RaycastHit hit))
        {
            Physics.Raycast(
                transform.position, hit.point - transform.position, out RaycastHit hit2, controller.height, ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
            );

            if (Vector3.Dot(hit2.normal, CharacterUpVector()) < 1 - dx && Vector3.Dot(hit2.normal, CharacterUpVector()) > dx)
            {
                verticalVelocity = Vector3.Cross(Vector3.Cross(CharacterUpVector(), hit2.normal), hit2.normal) * -VerticalSpeed;

            }
        }

        Vector3 combinedVelocity = verticalVelocity + moveVelocity;
        controller.Move(combinedVelocity * Time.deltaTime);
        return;
    }

    void UpdateState()
    {
        // land on ground
        if (State == MovementState.Airbourne && IsOnStableGround() && VerticalSpeed <= dx)
        {
            Landed?.Invoke();

            State = MovementState.Grounded;

            IsJumping = false;
            ExtraJumpsRemaining = Jumps - 1;

            return;
        }

        bool isGroundWithinSnappingDistance = Physics.Raycast(
            transform.position,
            -CharacterUpVector(),
            controller.height / 2 + controller.skinWidth + SNAPPING_DISTANCE,
            ControlConstants.RAYCAST_MASK);

        // when character lifts off
        if (State == MovementState.Grounded && (!isGroundWithinSnappingDistance || IsRising))
        {
            State = MovementState.Airbourne;
            return;
        }

        // snap to ground while walking
        if (State == MovementState.Grounded)
        {
            SnapCharacterToGround();
            return;
        }

        // when character falls from ledge
        if (!IsOnStableGround())
        {
            State = MovementState.Airbourne;
            return;
        }
    }

    void ApplyRotation()
    {
        Vector3 characterUp = CharacterUpVector();
        transform.LookAt(transform.position + FacingDirection(), characterUp);
    }

    void UpdateLastGrounded()
    {
        if (State == MovementState.Grounded)
        {
            LastTimeGrounded = Time.time;
        }
    }

    void SnapCharacterToGround()
    {
        bool isGroundWithinSnappingDistance = Physics.CapsuleCast(
            TopSphereCenter(), BottomSphereCenter(), controller.radius + controller.skinWidth - dx,
            -CharacterUpVector(),
            out RaycastHit capsuleHit, SNAPPING_DISTANCE, ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
        );

        if (!isGroundWithinSnappingDistance)
        {
            return;
        }

        bool isOnGround = Physics.CheckCapsule(
            TopSphereCenter(), BottomSphereCenter(), controller.radius + controller.skinWidth + dx, ControlConstants.RAYCAST_MASK, QueryTriggerInteraction.Ignore
        );

        if (isOnGround)
        {
            return;
        }

        transform.position += capsuleHit.distance * -CharacterUpVector();
    }

    #region Enable/Disable Features
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
    #endregion

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