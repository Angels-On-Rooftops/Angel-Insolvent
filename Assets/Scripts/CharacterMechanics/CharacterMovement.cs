using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum VerticalMovementState
{
    Falling,
    Grounded,
    Jumping,
}

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    [Tooltip(
        "The camera that the character's movement should be based off of. "
            + "Player movement is relative to this camera."
    )]
    public Camera Camera;

    [Space(20)]
    [SerializeField]
    [Tooltip(
        "The keybinds that control lateral character movement. To restrict character movement to a single axis, unbind the directions you don't want accessible."
    )]
    InputAction Walk;

    [SerializeField]
    [Min(0)]
    [Tooltip("The speed the character moves at in units/second.")]
    public float WalkSpeed = 16;

    [SerializeField]
    [Min(0)]
    [Tooltip(
        "Determines how fast the character falls relative to everything else in the scene. "
            + "A higher multiplier will result in a character that lifts off faster and falls faster."
    )]
    public float GravityMultiplier = 8;

    [Space(10)]
    [SerializeField]
    [Tooltip("The keybinds that make the character jump.")]
    public InputAction Jump;

    [SerializeField]
    [Tooltip("The maximum amount of jumps the character has before they touch the ground again.")]
    public int Jumps = 1;

    [SerializeField]
    [Min(0)]
    [Tooltip(
        "The maximum height of the character's jump in units. If gravity is changed, "
            + "the initial velocity of the jump will change accordingly to give the same jump height."
    )]
    public float JumpHeight = 3;

    [SerializeField]
    [Tooltip(
        "On subsequent midair jumps, the jump height will increment based off of this property.\n\n"
            + "For example, if the initial jump height is 5, the bonus is 5, and the number of jumps the character has is 3: "
            + "On the first jump the character will jump 5 units, then on the double jump it will jump 10, then on the triple jump it will jump 15."
    )]
    public float JumpHeightBonus = 0;

    [SerializeField]
    [Tooltip(
        "After pressing the jump key in mid air, the character will jump immediately "
            + "if they hit the ground within this timeframe (in seconds)."
    )]
    public float JumpBufferTime = 0.1f;

    [SerializeField]
    [Tooltip(
        "After leaving a ledge, if the player presses the jump key within "
            + "this timeframe, they will jump in midair. This is useful for when the "
            + "player wants to jump at the edge of a platform."
    )]
    public float CoyoteTime = 0.1f;

    [SerializeField]
    [Tooltip("The maximum speed the character can fall at.")]
    public float DownwardTerminalVelocity = 64;

    [Space(10)]
    [SerializeField]
    [Tooltip(
        "If enabled, to keep characters from floating when moving off ledges, they will snap "
            + "downwards when the ground slope changes."
    )]
    public bool SnapToGround = true;

    [SerializeField]
    [Min(0)]
    [Tooltip(
        "Determines the maximum distance the controller will check "
            + "for ground below the character before snapping it downward if SnapToGround is enabled."
    )]
    public float MaximumSnappingDistance = 0.2f;
    #endregion

    #region Events
    public event Action WalkStartRequested;
    public event Action WalkStopRequested;
    public event Action StartedWalking;
    public event Action StoppedWalking;
    public event Action<int> Jumped;
    public event Action JumpRequested;
    public event Action Landed;
    public event Action Falling;

    public event Action<Vector3> ImpulseApplied;
    public event Action JumpedFromImpulse;

    public event Action RanIntoWall; // TODO
    #endregion

    #region Public Properties
    CharacterController Controller => GetComponent<CharacterController>();
    Vector3 ControllerWorldPosition => transform.position + Controller.center;

    public Vector3 TopSphereCenter =>
        ControllerWorldPosition + GravityUpDirection * (Controller.height / 2 - Controller.radius);
    public Vector3 BottomSphereCenter =>
        ControllerWorldPosition - GravityUpDirection * (Controller.height / 2 - Controller.radius);

    public bool IsFalling => VerticalSpeed < 0;
    public bool IsRising => VerticalSpeed > 0;
    public bool LateralMovementEnabled { get; set; } = true;
    public bool RotationEnabled { get; set; } = true;
    public bool VerticalMovementEnabled
    {
        get { return GravityEnabled && StateChangeEnabled; }
        set
        {
            GravityEnabled = value;
            StateChangeEnabled = value;

            VerticalState = value ? VerticalMovementState.Falling : VerticalMovementState.Grounded;
            VerticalSpeed = 0;
        }
    }
    public bool GravityEnabled { get; set; } = true;
    public bool StateChangeEnabled { get; set; } = true;
    #endregion

    #region Private Properties
    float CharacterGravity => Physics.gravity.magnitude * GravityMultiplier;
    float TimeSinceGrounded => IsOnStableGround() ? 0 : Time.time - LastTimeGrounded;
    bool IsHittingHead =>
        Mathf.Abs(Vector3.Dot(Controller.velocity, GravityUpDirection)) < dx && IsRising;
    #endregion

    #region Constants
    const float dx = 0.01f;
    #endregion

    [NonSerialized]
    public Vector3 GravityUpDirection = new(0, 1, 0);

    //[NonSerialized]
    public float VerticalSpeed = 0;

    //[NonSerialized]
    public int ExtraJumpsRemaining = 0;

    public VerticalMovementState VerticalState = VerticalMovementState.Falling;

    float LastTimeGrounded = 0;

    #region Processed Vectors
    public Vector3 RawMovementDirection = new();
    public Vector3 RawFacingDirection = new();
    public Func<Vector3, float, Vector3> MovementDirectionMiddleware = (v, dt) => v;
    public Func<Vector3, float, Vector3> FacingDirectionMiddleware = (v, dt) => v;
    public Vector3 FacingDirection = new();
    public Vector3 MovementDirection = new();
    #endregion

    #region AdditionalImpulse
    [SerializeField]
    Vector3 StartingAdditionalImpulse = Vector3.zero;
    Vector3 CurrentAdditionalImpulse =>
        Vector3.Lerp(StartingAdditionalImpulse, Vector3.zero, Math.Min(1, PercentImpulseTimeLeft));

    [SerializeField]
    AnimationCurve ImpulseDecay;

    float ImpulseTime = 0;
    float TimeImpulseApplied = 0;
    float PercentImpulseTimeLeft =>
        ImpulseTime != 0 ? Mathf.Max((Time.time - TimeImpulseApplied) / ImpulseTime, 0) : 0;
    #endregion

    #region Platform Tracking
    // the last platform the character landed on
    GameObject LastPlatform;

    // the position of the last platform on the previous frame
    Vector3 LastPlatformLastPosition;

    // the velocity of the last platform on the previous frame
    Vector3 LastPlatformLastVelocity;
    float LastPlatformLastTime;
    #endregion

    readonly Maid maid = new();

    private void Awake()
    {
        MovementDirectionMiddleware = MovementMiddleware.RelativeToCamera(this);
        FacingDirectionMiddleware = FacingMiddleware.UpdateOnlyWhenMoving(this);
    }

    void OnEnable()
    {
        if (Walk is not null)
        {
            maid.GiveEvent(Walk, "performed", (CallbackContext c) => DoWalk(c));
            maid.GiveEvent(Walk, "performed", (CallbackContext c) => DoFacing(c));
            Walk.Enable();
        }

        if (Jump is not null)
        {
            maid.GiveEvent(Jump, "performed", (CallbackContext c) => DoJumpInput(c));
            Jump.Enable();
        }

        // instantly kills momentum when you touch the ground, using smoothdamp in Update() instead
        maid.GiveEvent(this, "Landed", () => StartingAdditionalImpulse.y = 0);
    }

    void OnDisable()
    {
        maid.Cleanup();
    }

    void DoWalk(CallbackContext c)
    {
        Vector3 oldRawMovementDirection = RawMovementDirection;

        Vector2 move2d = c.ReadValue<Vector2>();
        RawMovementDirection = new Vector3(move2d.x, 0, move2d.y);

        if (oldRawMovementDirection.magnitude == 0 && RawMovementDirection.magnitude != 0)
        {
            WalkStartRequested?.Invoke();
        }
        else if (oldRawMovementDirection.magnitude != 0 && RawMovementDirection.magnitude == 0)
        {
            WalkStopRequested?.Invoke();
        }
    }

    void DoFacing(CallbackContext c)
    {
        Vector2 newDirection = c.ReadValue<Vector2>();

        if (newDirection.magnitude > 0)
        {
            RawFacingDirection = new Vector3(newDirection.x, 0, newDirection.y).normalized;
        }
    }

    float JumpPower(float jumpHeight)
    {
        float height = jumpHeight + (Jumps - ExtraJumpsRemaining - 1) * JumpHeightBonus;
        return Mathf.Sqrt(2 * CharacterGravity * height);
    }

    void DoJump(float jumpBufferLeft, float scheduledJumpHeight)
    {
        bool FromGround = VerticalState == VerticalMovementState.Grounded;
        bool WithinCoyoteTime = TimeSinceGrounded <= CoyoteTime;
        bool NeedsExtraJump = !(
            (FromGround || WithinCoyoteTime) && VerticalState != VerticalMovementState.Jumping
        );

        // Reset extra jumps when grounded
        if (!NeedsExtraJump)
        {
            ExtraJumpsRemaining = Jumps - 1;
        }

        // Do actual jump
        if (!NeedsExtraJump || ExtraJumpsRemaining > 0)
        {
            if (NeedsExtraJump)
            {
                ExtraJumpsRemaining--;
            }

            VerticalSpeed = JumpPower(scheduledJumpHeight);
            VerticalState = VerticalMovementState.Jumping;
            Jumped?.Invoke(Jumps - ExtraJumpsRemaining);
        }
        else if (jumpBufferLeft > 0)
        {
            // jump didn't succeed, buffer it
            jumpBufferLeft -= Time.deltaTime;

            // if the jump height changes while buffered, we want to give the player the benefit
            // of the highest jump height they could've been aiming for
            StartCoroutine(
                CoroutineUtil.WaitAFrameThenRun(
                    () => DoJump(jumpBufferLeft, Mathf.Max(JumpHeight, scheduledJumpHeight))
                )
            );
        }
    }

    void DoJumpInput(CallbackContext _)
    {
        JumpRequested?.Invoke();
        DoJump(JumpBufferTime, JumpHeight);
    }

    public Vector3 ForwardMovementDirectionFromCamera()
    {
        // get the custom camera transform if we need to, otherwise the regular one will be fine
        Vector3 forward =
            Camera.TryGetComponent(out CharacterCamera PlayerCamera) && PlayerCamera.enabled
                ? PlayerCamera.GetNextCameraTransform().forward
                : Camera.transform.forward;

        return Vector3.Scale(-forward, Vector3.one - Vector3.up);
    }

    public Quaternion ForwardRotationFromCamera()
    {
        return Quaternion.LookRotation(-ForwardMovementDirectionFromCamera(), Vector3.up);
    }

    Vector3 MovementVelocity()
    {
        if (!LateralMovementEnabled)
        {
            return Vector3.zero;
        }

        return MovementDirection * WalkSpeed;
    }

    (bool didHit, RaycastHit hit) GroundInfo(float checkDistance)
    {
        bool didHit = Physics.CapsuleCast(
            BottomSphereCenter,
            TopSphereCenter,
            Controller.radius - dx,
            -GravityUpDirection,
            out RaycastHit hit,
            checkDistance,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        return (didHit, hit);
    }

    Vector3 GroundNormal(float checkDistance)
    {
        return GroundInfo(checkDistance).hit.normal;
    }

    float GroundAngle(float checkDistance)
    {
        bool didHit = Physics.CapsuleCast(
            BottomSphereCenter + dx * GravityUpDirection,
            TopSphereCenter,
            Controller.radius - dx,
            -GravityUpDirection,
            out RaycastHit hit,
            checkDistance,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );
        return didHit ? Vector3.Angle(hit.normal, GravityUpDirection) : 90;
    }

    float PointGroundAngle(float checkDistance)
    {
        bool didHit = Physics.Raycast(
            BottomSphereCenter + dx * GravityUpDirection,
            -GravityUpDirection,
            out RaycastHit hit,
            checkDistance,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );
        return didHit ? Vector3.Angle(hit.normal, GravityUpDirection) : 90;
    }

    public bool IsOnStableGround()
    {
        if (GroundAngle(Controller.height / 2f + dx) > Controller.slopeLimit)
        {
            return false;
        }

        return IsOnGround();
    }

    public bool IsOnGround()
    {
        bool didHit = Physics.CheckSphere(
            BottomSphereCenter - GravityUpDirection * (Controller.skinWidth + 2 * dx),
            Controller.radius,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        return didHit;
    }

    public bool IsOnSteepSlope()
    {
        return !IsOnStableGround() && IsOnGround();
    }

    void AddToVerticalSpeed(float toAdd)
    {
        VerticalSpeed = Math.Max(VerticalSpeed + toAdd, -DownwardTerminalVelocity);
    }

    void ApplyGravity()
    {
        if (
            (VerticalState == VerticalMovementState.Grounded && VerticalSpeed < dx)
            || Mathf.Abs(CurrentAdditionalImpulse.y) > dx
        )
        {
            VerticalSpeed = 0;
            return;
        }

        AddToVerticalSpeed(-CharacterGravity * Time.deltaTime);
    }

    void ApplyMovementVelocity(Vector3 additionalImpulse)
    {
        Vector3 moveVelocity = MovementVelocity();

        Physics.CapsuleCast(
            BottomSphereCenter,
            TopSphereCenter,
            Controller.radius,
            -GravityUpDirection,
            out RaycastHit hit,
            Controller.height / 2,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        // subtract the component of the move velocity that's going up too steep of a slope
        (Vector3 acrossSlope, Vector3 downSlope) = VectorMath.BasisVectorsOfSlope(
            hit.normal,
            GravityUpDirection
        );
        Vector3 DownSlopeDirection = downSlope.normalized;

        // subtract the component of the move velocity that's going up too steep of a slope
        //if (
        //    PointGroundAngle(Controller.height / 2f + dx) > Controller.slopeLimit
        //    && Vector3.Dot(FacingDirection, downSlope) > 0
        //    && PointGroundAngle(Controller.height / 2f + dx) != 90
        //)
        //{
        //    // if using 2022.3.4
        //    //moveVelocity -= Vector3.Project(
        //    //    moveVelocity, Vector3.ProjectOnPlane(DownSlopeDirection, Vector3.one - Vector3.up)
        //    //);

        //    // if using 2022.3.47+
        //    moveVelocity -= Vector3.Project(
        //        moveVelocity,
        //        Vector3.Scale(
        //            Vector3.up * 100,
        //            Vector3.ProjectOnPlane(DownSlopeDirection, Vector3.one - Vector3.up)
        //        )
        //    );

        //    // if using between those versions, i'm not sure
        //}

        //// make character slip off edge to prevent being stuck on the very edge of a platform
        //if (IsOnSteepSlope() && GroundNormal(Controller.height).magnitude == 0)
        //{
        //    moveVelocity += FacingDirection;
        //}

        if (!IsOnGround() && additionalImpulse.magnitude != 0)
        {
            moveVelocity -=
                ImpulseDecay.Evaluate(1 - PercentImpulseTimeLeft)
                * Vector3.Dot(additionalImpulse.normalized, moveVelocity)
                * additionalImpulse.normalized;
        }

        // redirect vertical speed down the slope if moving downwards
        Vector3 verticalDirection =
            (IsOnSteepSlope() && VerticalSpeed < 0) ? DownSlopeDirection : GravityUpDirection;
        Vector3 verticalVelocity = verticalDirection * VerticalSpeed;

        // bring it all together
        Vector3 combinedVelocity = verticalVelocity + moveVelocity + new Vector3(0, -dx, 0);

        Controller.Move((combinedVelocity + additionalImpulse) * Time.deltaTime);
    }

    void ApplyRotation()
    {
        transform.LookAt(transform.position + FacingDirection, GravityUpDirection);
    }

    void UpdateProcessedVectors()
    {
        Vector3 oldMovementVector = MovementDirection;

        MovementDirection = MovementDirectionMiddleware(RawMovementDirection, Time.deltaTime);
        FacingDirection = FacingDirectionMiddleware(RawFacingDirection, Time.deltaTime);

        if (oldMovementVector.magnitude == 0 && MovementDirection.magnitude != 0)
        {
            StartedWalking?.Invoke();
        }
        else if (oldMovementVector.magnitude != 0 && MovementDirection.magnitude == 0)
        {
            StoppedWalking?.Invoke();
        }
    }

    void UpdateState()
    {
        // Jumping -> Falling
        if (VerticalState == VerticalMovementState.Jumping && VerticalSpeed < dx)
        {
            VerticalState = VerticalMovementState.Falling;
            Falling?.Invoke();
            return;
        }

        // Falling -> Grounded
        if (VerticalState == VerticalMovementState.Falling && IsOnStableGround())
        {
            VerticalState = VerticalMovementState.Grounded;
            Landed?.Invoke();
            return;
        }

        // Grounded -> Falling
        if (VerticalState == VerticalMovementState.Grounded && !IsOnStableGround())
        {
            bool isGroundWithinSnappingDistance = Physics.Raycast(
                transform.position,
                -GravityUpDirection,
                Controller.height / 2 + Controller.skinWidth + MaximumSnappingDistance,
                ControlConstants.RAYCAST_MASK
            );

            // Snap to ground if we can, if character is moving upwards we don't want to snap them back down
            if (
                SnapToGround
                && isGroundWithinSnappingDistance
                && !IsRising
                && GroundAngle(MaximumSnappingDistance) < Controller.slopeLimit
            )
            {
                SnapCharacterToGround();
                return;
            }

            // else start falling
            VerticalState = VerticalMovementState.Falling;
            Falling?.Invoke();
            return;
        }
    }

    void UpdateLastGrounded()
    {
        if (VerticalState == VerticalMovementState.Grounded)
        {
            LastTimeGrounded = Time.time;
        }
    }

    void SnapCharacterToGround()
    {
        bool didCapsuleHit = Physics.CapsuleCast(
            TopSphereCenter,
            BottomSphereCenter,
            Controller.radius + Controller.skinWidth - dx,
            -GravityUpDirection,
            out RaycastHit capsuleHit,
            MaximumSnappingDistance,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        bool isCapsuleOverGeometry = Physics.CheckCapsule(
            TopSphereCenter,
            BottomSphereCenter - GravityUpDirection * (Controller.skinWidth + dx),
            Controller.radius,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        if (didCapsuleHit & !isCapsuleOverGeometry)
        {
            transform.position += capsuleHit.distance * -GravityUpDirection;
        }
    }

    public void Warp(Vector3 position)
    {
        Controller.enabled = false;
        transform.position = position;
        Controller.enabled = true;
    }

    public void GiveImpulse(
        Vector3 newImpulse,
        float impulseTime,
        VerticalMovementState putIntoState = VerticalMovementState.Falling
    )
    {
        StartingAdditionalImpulse = newImpulse;
        TimeImpulseApplied = Time.time;
        ImpulseTime = impulseTime;

        if (newImpulse.y > dx)
        {
            VerticalState = putIntoState;
            Debug.Log($"put into state {VerticalState}");
            JumpedFromImpulse?.Invoke();
        }

        ImpulseApplied?.Invoke(newImpulse);
    }

    public void ForceHitWall()
    {
        RanIntoWall?.Invoke();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        UpdateProcessedVectors();

        if (GravityEnabled)
        {
            ApplyGravity();
        }

        ApplyMovementVelocity(CurrentAdditionalImpulse);

        if (RotationEnabled)
        {
            ApplyRotation();
        }

        if (StateChangeEnabled)
        {
            UpdateState();
        }

        UpdateLastGrounded();

        if (IsHittingHead)
        {
            VerticalSpeed = 0;
        }
    }
}
