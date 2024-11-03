using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    public float RollingSpeed = 24;

    [SerializeField]
    public float RollDuration = 1;

    [SerializeField]
    public float HighJumpWindow = 0.25f;

    [SerializeField]
    float ColliderHeight = 1;

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.LongJumping, jumped && !canHighJump },
            { AdvancedMovementState.Plunging, pushedActionButton && !Movement.IsOnGround() },
            {
                AdvancedMovementState.Decelerating,
                (IsRollOver() && Movement.IsOnGround()) || hitWall
            },
            { AdvancedMovementState.HighJumping, jumped && canHighJump },
        };

    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "JumpHeight", LongJumpHeight },
            { "MovementDirectionMiddleware", MovementMiddleware.FullSpeedAhead(Movement, 3.5f) },
            { "FacingDirectionMiddleware", FacingMiddleware.FaceMovementDirection(Movement) },
            { "Jumps", 2 },
            { "ExtraJumpsRemaining", 1 },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    public Vector3 RollingDirection = Vector3.zero;

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    float HighJumpHeight => GetComponent<HighJump>().HighJumpHeight;
    float LongJumpHeight => GetComponent<LongJump>().LongJumpHeight;

    readonly Maid StateMaid = new();

    bool jumped = false;
    bool pushedActionButton = false;
    bool canHighJump = false;
    float timeStarted;
    bool hitWall = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        // zero the rolling vector so we know to take it from the movement controller
        RollingDirection = Vector3.zero;

        timeStarted = Time.time;

        if (Movement.WalkSpeed < RollingSpeed)
        {
            Movement.WalkSpeed = RollingSpeed;
        }

        jumped = false;
        StateMaid.GiveEvent(Movement, "Jumped", (int _) => jumped = true);

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);

        canHighJump = false;
        Coroutine highJumpCounter = null;
        if (fromState == AdvancedMovementState.Plunging)
        {
            canHighJump = true;
            Movement.JumpHeight = HighJumpHeight;
            highJumpCounter = StartCoroutine(
                CoroutineUtil.DoActionAfterTime(
                    () =>
                    {
                        canHighJump = false;
                        Movement.JumpHeight = LongJumpHeight;
                    },
                    HighJumpWindow
                )
            );
        }
        StateMaid.GiveTask(() =>
        {
            if (highJumpCounter is not null)
            {
                StopCoroutine(highJumpCounter);
            }
        });

        StateMaid.GiveTask(AdvancedMovement.SetColliderHeight(ColliderHeight));
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    bool IsRollOver()
    {
        return Time.time - timeStarted >= RollDuration;
    }

    void LateUpdate()
    {
        // unstick the action button, so if the roll was still on the ground
        // it won't immediately dive when rolling off an edge
        // Note: has to be done on LateUpdate because state transitions
        //       take place in Update
        pushedActionButton = false;
    }
}
